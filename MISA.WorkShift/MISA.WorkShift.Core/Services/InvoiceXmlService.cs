using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MISA.WorkShift.Core.Services
{
    public class InvoiceXmlService : IInvoiceXmlService
    {
        public XDocument BuildInvoiceXml(Invoice invoice, List<InvoiceDetail> details)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string fakeId = "MISA_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Tạo Object XDocument
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("HDon",
                    new XElement("DLHDon", new XAttribute("Id", fakeId),
                        new XElement("TTChung",
                            new XElement("PBan", "2.0.1"),
                            new XElement("THDon", invoice.InvoiceType),
                            new XElement("SHDon", invoice.InvoiceNumber),
                            new XElement("NLap", invoice.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss", culture)),
                            new XElement("DVTTe", invoice.CurrencyCode ?? "VND"),
                            new XElement("TGia", invoice.ExchangeRate.ToString(culture)),
                            new XElement("HTTToan", invoice.PaymentMethod ?? string.Empty)
                        ),
                        new XElement("NBan",
                            new XElement("MST", invoice.SellerTaxCode),
                            new XElement("Ten", invoice.SellerName)
                        ),
                        new XElement("NMua",
                            new XElement("Ten", invoice.BuyerLegalName),
                            new XElement("MST", invoice.BuyerTaxCode)
                        ),
                        new XElement("DSHHDVu",
                            details?.Select((d, i) => new XElement("HHDVu",
                                new XElement("STT", i + 1),
                                new XElement("THHDVu", d.ItemName),
                                new XElement("SLuong", d.Quantity.ToString(culture)),
                                new XElement("DGia", d.UnitPrice.ToString(culture)),
                                new XElement("TSuat", d.TaxRate + "%"),
                                new XElement("TTien", d.Amount.ToString(culture))
                            ))
                        ),
                        new XElement("TToan",
                            new XElement("TgTCien", invoice.TotalAmount.ToString(culture)),
                            new XElement("TgTienBangChu", invoice.TotalAmountInWords)
                        )
                    ),
                    new XElement("DSCKS", new XElement("NBan")) // Placeholder cho MISA
                )
            );

            return doc;
        }
        public string SaveInvoiceXml(Invoice invoice, XDocument xml)
        {
            var appRoot = Directory.GetCurrentDirectory();
            var privateRoot = Path.Combine(appRoot, "private_storage", "invoices", invoice.CompanyId.ToString());
            Directory.CreateDirectory(privateRoot);

            var fileName = invoice.InvoiceId + ".xml";
            var savedFullPath = Path.Combine(privateRoot, fileName);
            xml.Save(savedFullPath);

            return Path.Combine("private_storage", "invoices", invoice.CompanyId.ToString(), fileName).Replace('\\', '/');
        }

        public string UpdateInvoiceXml(Invoice invoice, List<InvoiceDetail> details)
        {
            var xml = BuildInvoiceXml(invoice, details);
            return SaveInvoiceXml(invoice, xml);
        }

        public void DeleteInvoiceXml(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var p = relativePath.StartsWith("/") ? relativePath.Substring(1) : relativePath;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), p.Replace('/', Path.DirectorySeparatorChar));
            try
            {
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch
            {
                // swallow
            }
        }
    }
}
