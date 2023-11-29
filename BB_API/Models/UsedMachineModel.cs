using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UsedMachineModel
    {
        public  class BB_Maquinas_Usadas_Gestor_Detail
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string DVCT { get; set; }

            public string DVCTRegistada { get; set; }

            public bool? CatCancelado { get; set; }

            public string Armazem { get; set; }

            public DateTime? Data_Entrada { get; set; }

            public string ClienteNr { get; set; }

            public string ClienteNome { get; set; }

            public string Observacoes { get; set; }

            public string Comentarios { get; set; }

            public double? PriceUsed { get; set; }

            public string Modelo { get; set; }


        }


        public class BB_Maquinas_Usadas_Logisitca_Detail_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string DVCT { get; set; }

            public string DVCTRegistada { get; set; }

            public bool? CatCancelado { get; set; }

            public string Armazem { get; set; }

            public DateTime? Data_Entrada { get; set; }

            public string ClienteNr { get; set; }

            public string ClienteNome { get; set; }

            public string Observacoes { get; set; }

            public string Comentarios { get; set; }

            public double? PriceUsed { get; set; }

            public string Modelo { get; set; }


        }

        public class BB_Maquinas_Usadas_Tecnico_Detail
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string DVCT { get; set; }

            public string DVCTRegistada { get; set; }

            public bool? CatCancelado { get; set; }

            public string Armazem { get; set; }

            public DateTime? Data_Entrada { get; set; }

            public string ClienteNr { get; set; }

            public string ClienteNome { get; set; }

            public string Observacoes { get; set; }

            public int? Status { get; set; }

            public string Comentarios { get; set; }

            public double? PriceUsed { get; set; }

            public string PontoEnvio_Morada_Recolha { get; set; }
            public string PontoEnvio_Contacto_Recolha { get; set; }
            public string PontoEnvio_Telefone_Recolha { get; set; }
            public string ContadorBW_Antigo { get; set; }
            public string ContadorCOR_Antigo { get; set; }
            public string Recolha_Opcionais { get; set; }
            public string PontoEnvio_CodPostal_Recolha { get; set; }
            public string PontoEnvio_Localidade_Recolha { get; set; }
            public Nullable<bool> Leasedesk { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }

            public string PS_Recolha { get; set; }

            public string Modelo { get; set; }

            public string accao { get; set; }

        }

       

        public partial class BB_Maquinas_Usadas_Gestor_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ClienteNr { get; set; }

            public string Modelo { get; set; }

            public string Comentarios { get; set; }

            public double? PriceUsed { get; set; }
        }

        public partial class BB_Maquinas_Usadas_Logistica_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ClienteNr { get; set; }

            public string Modelo { get; set; }

            public string Comentarios { get; set; }

            public double? PriceUsed { get; set; }
        }

        public class BB_Maquinas_Usadas_Logistica_Detail
        {
            public int ID { get; set; }
            public string NUS_DVCT { get; set; }
            public string NUS_DVCT_REGISTADA { get; set; }
            public Nullable<bool> Cat_Cancelado { get; set; }
            public string NUS_PS { get; set; }
            public string Nr_Entrada_Armazem { get; set; }
            public string NUS_Referencia { get; set; }
            public string ARM { get; set; }
            public string NUS_Modelo { get; set; }
            public string NUS_Tipo { get; set; }
            public string NUS_Nr_Serie { get; set; }
            public string Observacoes_Logistica { get; set; }
            public Nullable<System.DateTime> NUS_Data_Entrada_Equip { get; set; }
            public Nullable<int> Contador_Preto { get; set; }
            public Nullable<int> Contador_Cor { get; set; }
            public Nullable<int> Contador_Total { get; set; }
            public string Pedido_Por { get; set; }
            public bool Validar { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ReservedUser { get; set; }
            public Nullable<double> PriceUsed { get; set; }

            public DateTime? dateTimeCreated { get; set; }
            public DateTime? dateTimeExpire { get; set; }

            public string accao { get; set; }

            public string ClienteNome { get; set; }

        }
        public partial class BB_Maquinas_Usadas_Pedidos_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ClienteNr { get; set; }

            public string Modelo { get; set; }

            public string Comentarios { get; set; }

            public int? Status { get; set; }

            public string ClienteNome { get; set; }

            public string Observacoes { get; set; }

            public string DVCT { get; set; }

            public string DVCTRegistada { get; set; }

            public bool? CatCancelado { get; set; }

            public string Armazem { get; set; }

            public DateTime? Data_Entrada { get; set; }

            public double? PriceUsed { get; set; }
        }

        public class BB_Maquinas_Usadas_Tecnico_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ClienteNr { get; set; }

            public string Modelo { get; set; }

            public string Comentarios { get; set; }

            public int? Status { get; set; }
            public double? PriceUsed { get; set; }

            public bool? Leasedesk { get; set; }

            public DateTime? ModifiedDate { get; set; }

            public string RequestTo { get; set; }
        }

        public partial class BB_Maquinas_Usadas_Model_
        {
            public int ID { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public string ReservedBy { get; set; }
            public Nullable<System.DateTime> ReservedDateTime { get; set; }
            public Nullable<System.DateTime> ExpireDate { get; set; }
            public string PS_NUS { get; set; }
            public string Codref { get; set; }
            public string Type_MFP { get; set; }
            public string NrSerie { get; set; }
            public Nullable<int> NrCopyBW { get; set; }
            public Nullable<int> NrCopyCOLOR { get; set; }
            public Nullable<bool> IsReserved { get; set; }
            public string ClienteNr { get; set; }

            public string Modelo { get; set; }

            public string Comentarios { get; set; }

            public int? Status { get; set; }

            public string ClienteNome { get; set; }

            public string Observacoes { get; set; }

            public string DVCT { get; set; }

            public string DVCTRegistada { get; set; }

            public bool? CatCancelado { get; set; }

            public string Armazem { get; set; }

            public DateTime? Data_Entrada { get; set; }

            public string StatusName { get; set; }

            public int? TypeT { get; set; }

            public string ReservedBy_Gestor { get; set; }

            public bool? IsReserved_Gestor { get; set; }
            public DateTime? ExpireDate_Gestor { get; set; }

            public DateTime? ReservedDateTime_Gestor { get; set; }
            public string ReservedBy_Tecnico { get; set; }
            public bool? IsReserved_Tecnico { get; set; }
            public DateTime? ExpireDate_Tecnico { get; set; }

            public DateTime? ReservedDateTime_Tecnico { get; set; }
            public string ClientNr_Tecnico { get; set; }

            public double? PriceUsed { get; set; }
        }
    }
}