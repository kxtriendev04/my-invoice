using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MISA.WorkShift.Api.Services
{
    public class TemplateFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TemplateFileService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // Get list of default templates available in wwwroot/templates/defaults
        // Reads optional metadata file 'templates.json' to provide display name and preview image
        public IEnumerable<DefaultTemplateInfo> GetDefaultTemplates()
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(webRoot, "templates", "defaults");
            if (!Directory.Exists(dir)) return Array.Empty<DefaultTemplateInfo>();

            var files = Directory.GetFiles(dir, "*.xslt");
            var detected = files.Select(f => new DefaultTemplateInfo
            {
                FileName = Path.GetFileName(f),
                TemplateCode = Path.GetFileNameWithoutExtension(f),
                RelativePath = "/" + Path.GetRelativePath(webRoot, f).Replace("\\", "/")
            }).ToList();

            // Try to read metadata JSON to enrich info
            var metaPath = Path.Combine(dir, "templates.json");
            if (!File.Exists(metaPath)) return detected;

            try
            {
                var json = File.ReadAllText(metaPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var metas = JsonSerializer.Deserialize<List<DefaultTemplateMetadata>>(json, options) ?? new List<DefaultTemplateMetadata>();

                // Merge metadata with detected files, metadata wins for display name/preview
                var result = new List<DefaultTemplateInfo>();
                // index detected by code
                var detectedMap = detected.ToDictionary(d => d.TemplateCode, StringComparer.OrdinalIgnoreCase);

                foreach (var m in metas)
                {
                    if (detectedMap.TryGetValue(m.TemplateCode, out var found))
                    {
                        found.DisplayName = m.DisplayName ?? found.FileName;
                        if (!string.IsNullOrEmpty(m.PreviewRelativePath)) found.PreviewRelativePath = m.PreviewRelativePath;
                        if (!string.IsNullOrEmpty(m.Category)) found.Category = m.Category;
                        if (!string.IsNullOrEmpty(m.InvoiceType)) found.InvoiceType = m.InvoiceType;
                        if (m.Sizes != null && m.Sizes.Any()) found.Sizes = m.Sizes;
                        if (m.TaxRates != null && m.TaxRates.Any()) found.TaxRates = m.TaxRates;
                        result.Add(found);
                        detectedMap.Remove(m.TemplateCode);
                    }
                    else
                    {
                        // metadata for missing xslt - still include if file path present
                        var rel = m.XsltRelativePath;
                        if (!string.IsNullOrEmpty(rel))
                        {
                            var ti = new DefaultTemplateInfo
                            {
                                TemplateCode = m.TemplateCode,
                                FileName = Path.GetFileName(rel),
                                RelativePath = rel,
                                DisplayName = m.DisplayName ?? m.TemplateCode,
                                PreviewRelativePath = m.PreviewRelativePath,
                                Category = m.Category,
                                InvoiceType = m.InvoiceType,
                                Sizes = m.Sizes ?? new List<string>(),
                                TaxRates = m.TaxRates ?? new List<string>()
                            };
                            result.Add(ti);
                        }
                    }
                }

                // add any remaining detected templates without metadata
                result.AddRange(detectedMap.Values.OrderBy(d => d.FileName));

                return result;
            }
            catch
            {
                return detected;
            }
        }

        // Save user-specific xslt content to filesystem and return relative path
        public string SaveXsltToFile(Guid companyId, Guid userId, string xsltContent, string fileName = null)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var root = Path.Combine(webRoot, "templates");
            var dir = Path.Combine(root, companyId.ToString(), userId.ToString());
            Directory.CreateDirectory(dir);
            fileName ??= $"template_{DateTime.Now:yyyyMMddHHmmss}.xslt";
            var path = Path.Combine(dir, fileName);
            File.WriteAllText(path, xsltContent);
            // return relative path from web root
            var relative = Path.GetRelativePath(webRoot, path).Replace("\\", "/");
            return "/" + relative;
        }

        // Read default template from defaults folder by template code
        public string? ReadDefaultXslt(string templateCode)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var path = Path.Combine(webRoot, "templates", "defaults", templateCode + ".xslt");
            if (!File.Exists(path)) return null;
            return File.ReadAllText(path);
        }

        // Read a user instance file by relative path
        public string? ReadXsltFromFilePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var p = relativePath.StartsWith("/") ? relativePath.Substring(1) : relativePath;
            var path = Path.Combine(webRoot, p.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path)) return null;
            return File.ReadAllText(path);
        }

        // Read any file (image, etc) by relative path and return data URI (base64)
        public string? ReadFileAsBase64(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var p = relativePath.StartsWith("/") ? relativePath.Substring(1) : relativePath;
            var path = Path.Combine(webRoot, p.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path)) return null;
            var bytes = File.ReadAllBytes(path);
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var mime = ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
            return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }
    }

    public class DefaultTemplateInfo
    {
        public string TemplateCode { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string PreviewRelativePath { get; set; } = string.Empty;
        // e.g. "invoice", "ticket", "receipt"
        public string Category { get; set; } = string.Empty;
        // e.g. "VAT", "single-rate", "multi-rate"
        public string InvoiceType { get; set; } = string.Empty;
        // supported sizes, e.g. ["A4","A5"]
        public List<string> Sizes { get; set; } = new List<string>();
        // tax rates applicable, e.g. ["10","8","0"]
        public List<string> TaxRates { get; set; } = new List<string>();
    }

    public class DefaultTemplateMetadata
    {
        public string TemplateCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        // path relative to web root, e.g. "/templates/defaults/preview1.png"
        public string PreviewRelativePath { get; set; } = string.Empty;
        // optional xslt relative path if xslt filename differs or stored elsewhere
        public string XsltRelativePath { get; set; } = string.Empty;
        // Category: invoice / ticket / receipt
        public string Category { get; set; } = string.Empty;
        // Invoice type: VAT / single-rate / multi-rate
        public string InvoiceType { get; set; } = string.Empty;
        // Sizes supported, e.g. ["A4","A5"]
        public List<string> Sizes { get; set; } = new List<string>();
        // Tax rates applicable, can be strings like "10", "8", "0"
        public List<string> TaxRates { get; set; } = new List<string>();
    }
}
