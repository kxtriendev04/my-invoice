using MISA.WorkShift.Core.Entities;
using System.Collections.Generic;

namespace MISA.WorkShift.Core.DTOs
{
    public class RegistrationSaveDto
    {
        public InvoiceRegistration Registration { get; set; }
        public List<RegDigitalSignature> Signatures { get; set; }
        public List<RegTVAN> Tvans { get; set; }
        public List<RegTransmissionUnit> TransmissionUnits { get; set; }
    }
}