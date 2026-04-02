using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MISA.WorkShift.Core.Services
{
    public class InvoiceXmlService : IInvoiceXmlService
    {
        public XDocument BuildInvoiceXml(Invoice invoice, List<InvoiceDetail> details)
        {
            // Build XML following TCT tag mapping (HDon -> DLHDon)
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            var hdon = new XElement("HDon");
            var dlhdon = new XElement("DLHDon");

            // TTChung block (general invoice info)
            var ttchung = new XElement("TTChung",
                new XElement("THDon", invoice.InvoiceType), // invoice type
                new XElement("KHMSHDon", invoice.TemplateCode ?? string.Empty), // template code
                new XElement("KHHDon", invoice.Series ?? string.Empty), // series
                new XElement("SHDon", invoice.InvoiceNumber ?? string.Empty), // invoice number
                new XElement("NLap", invoice.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss", culture)), // created date
                new XElement("DVTTe", invoice.CurrencyCode ?? "VND"),
                new XElement("TGia", invoice.ExchangeRate.ToString(culture)),
                new XElement("HTTToan", invoice.PaymentMethod ?? string.Empty)
            );

            // NBan (seller) block
            var nban = new XElement("NBan",
                new XElement("MST", invoice.SellerTaxCode ?? string.Empty),
                new XElement("Ten", invoice.SellerName ?? string.Empty),
                new XElement("DChi", invoice.SellerAddress ?? string.Empty)
            );

            // NMua (buyer) block
            var nmua = new XElement("NMua",
                new XElement("HVTNMHang", invoice.BuyerName ?? string.Empty),
                new XElement("Ten", invoice.BuyerLegalName ?? string.Empty),
                new XElement("MST", invoice.BuyerTaxCode ?? string.Empty),
                new XElement("DChi", invoice.BuyerAddress ?? string.Empty),
                new XElement("DCTDTu", invoice.BuyerEmail ?? string.Empty),
                new XElement("SDThoai", invoice.BuyerPhone ?? string.Empty),
                new XElement("STKhoan", invoice.BuyerBankAccount ?? string.Empty),
                new XElement("TNHang", invoice.BuyerBankName ?? string.Empty)
            );

            ttchung.Add(nban);
            ttchung.Add(nmua);

            // DS HHDVu (details)
            var dshhdvu = new XElement("DSHHDVu");
            if (details != null)
            {
                int stt = 1;
                foreach (var d in details)
                {
                    // map tax rate int to string
                    string tsuat = string.Empty;
                    if (d.TaxRate.HasValue)
                    {
                        switch (d.TaxRate.Value)
                        {
                            case 0: tsuat = "0%"; break;
                            case 5: tsuat = "5%"; break;
                            case 8: tsuat = "8%"; break;
                            case 10: tsuat = "10%"; break;
                            default: tsuat = d.TaxRate.Value + "%"; break;
                        }
                    }

                    var hhdvu = new XElement("HHDVu",
                        new XElement("STT", stt++),
                        new XElement("TChat", 1), // default to normal item
                        new XElement("THHDVu", d.ItemName ?? string.Empty),
                        new XElement("DVTinh", d.UnitName ?? string.Empty),
                        new XElement("SLuong", d.Quantity.ToString(culture)),
                        new XElement("DGia", d.UnitPrice.ToString(culture)),
                        new XElement("TLCKhau", d.DiscountRate.ToString(culture)),
                        new XElement("STCKhau", d.DiscountAmount.ToString(culture)),
                        new XElement("ThTien", (d.Quantity * d.UnitPrice - d.DiscountAmount).ToString(culture)),
                        new XElement("TSuat", tsuat),
                        new XElement("TienThue", d.TaxAmount.ToString(culture)),
                        new XElement("TTien", d.Amount.ToString(culture))
                    );

                    dshhdvu.Add(hhdvu);
                }
            }

            // TToan (totals)
            var ttoan = new XElement("TToan",
                new XElement("THTToan", invoice.TotalBeforeTax.ToString(culture)),
                new XElement("TTCKKhac", invoice.TotalDiscount.ToString(culture)),
                new XElement("TGTGT", invoice.TotalTaxAmount.ToString(culture)),
                new XElement("TgTCien", invoice.TotalAmount.ToString(culture)),
                new XElement("TgTienBangChu", invoice.TotalAmountInWords ?? string.Empty)
            );

            dlhdon.Add(ttchung);
            dlhdon.Add(dshhdvu);
            dlhdon.Add(ttoan);

            hdon.Add(dlhdon);

            // DSCKS placeholder (signatures) and MCCQT (tax office code)
            var dscKS = new XElement("DSCKS");
            hdon.Add(dscKS);

            var mccqt = new XElement("MCCQT", invoice.CqtCode ?? string.Empty);
            hdon.Add(mccqt);

            doc.Add(hdon);
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
