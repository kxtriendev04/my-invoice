using MISA.WorkShift.Core.Entities;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MISA.WorkShift.Core.Interfaces.Services
{
    public interface IInvoiceXmlService
    {
        XDocument BuildInvoiceXml(Invoice invoice, List<InvoiceDetail> details);

        /// <summary>
        /// Save XML to private storage and return relative path (using '/').
        /// Overwrites existing file if present.
        /// </summary>
        string SaveInvoiceXml(Invoice invoice, XDocument xml);

        /// <summary>
        /// Build and save XML for invoice (overwrite if exists) and return relative path.
        /// </summary>
        string UpdateInvoiceXml(Invoice invoice, List<InvoiceDetail> details);

        /// <summary>
        /// Delete XML file by relative path (best-effort).
        /// </summary>
        void DeleteInvoiceXml(string relativePath);
    }
}
