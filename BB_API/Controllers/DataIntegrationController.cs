using AutoMapper;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Provider;
//using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;

namespace WebApplication1.Controllers
{

    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DataIntegrationController : ApiController
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public masterEntities usersDB = new masterEntities();

        public BB_DB_DEV_LeaseDesk dbLeaseDesk = new BB_DB_DEV_LeaseDesk();

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDataIntegration")]
        public List<DIItem> GetDataIntegration()
        {
            //List<BB_Data_Integration> lst_BBDI = db.BB_Data_Integration.ToList();
            //return db.BB_Data_Integration.Take(10).Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();
            //return lst_BBDI.Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();
            return db.BB_Data_Integration.Select(x => new DIItem { ID = x.ID, CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU, BOM = x.BOM }).ToList();
        }

        public BB_Data_Integration Get(int id)
        {
            return db.BB_Data_Integration.FirstOrDefault(e => e.ID == id);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetQuotes")]
        public List<BB_CRM_Quotes_M> GetQuotes([FromBody] Owner_ Owner)
        {
            AspNetUsers user = usersDB.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            return db.BB_CRM_Quotes.Where(e => e.owneridname == user.DisplayName && e.statecodename == "Draft").Select(x => new BB_CRM_Quotes_M { quotenumber = x.quotenumber, accountnumber = x.accountnumber }).ToList<BB_CRM_Quotes_M>();
            //return db.BB_CRM_Quotes.Where(e => e.owneridname == user.DisplayName && e.statecodename == "Draft").ToList(); new City{Id = cty.Id, Name = cty.Name }).ToList<City>();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCampanhas")]
        public List<Campanhas> GetCampanhas()
        {
            List<Campanhas> campanhas = new List<Campanhas>();
            List<BB_Campanha> campanhasDB = db.BB_Campanha.ToList();

            foreach (var campanha in campanhasDB)
            {
                Campanhas c = new Campanhas();
                c.baskets = new Baskets();
                c.baskets.os_basket = new List<OsBasket>();

                c.baskets.rs_basket = new List<RsBasket>();

                c.ID = campanha.ID;
                c.Campanha = campanha.Campanha;


                if (campanha.ID == 1)
                {


                    OsBasket osbasket = new OsBasket();
                    osbasket.CodeRef = "ACER021";
                    osbasket.PVP = 492.50;
                    osbasket.Family = "OPSHW";
                    osbasket.Description = "Bizhub 4020i - inclui RADF, controlador PS/PCL,  unidade duplex, cassete de papel (250 folhas), bypass 50 fls, toner inicial (8Kpágs) & unidade de imagem";
                    osbasket.IsMarginBEU = true;
                    osbasket.Qty = 1;
                    osbasket.TCP = 7.5;
                    osbasket.ClickPriceBW = 0.0125;
                    osbasket.ClickPriceC = null;
                    c.baskets.os_basket.Add(osbasket);

                    RsBasket rsbasket = new RsBasket();
                    rsbasket.CodeRef = "9969PT00105";
                    rsbasket.PVP = 1;
                    rsbasket.Family = "OPSPSV";
                    rsbasket.Description = "Service Pack OPS Business (preço por equipamento/mês) 1 equipamento";
                    rsbasket.Qty = 1;
                    rsbasket.DiscountPercentage = 100;
                    rsbasket.TotalMonths = 12;
                    c.baskets.rs_basket.Add(rsbasket);


                    c.printingServices = new PrintingServices();
                    c.printingServices.modeId = 2;
                    c.printingServices.ContractPeriod = 0;
                    c.printingServices.FeeFrequency = 0;
                    c.printingServices.BillingFrequency = 0;

                    c.printingServices.vva = new Vva();
                    c.printingServices.vva.MonthlyFee = 7.5;
                    c.printingServices.vva.CVolume = 0;
                    c.printingServices.vva.BWVolume = 500;
                    c.printingServices.vva.BWExcess = 0.009;
                    c.printingServices.vva.UseGlobalExcess = true;
                    c.printingServices.vva.CExcess = 0;
                    c.printingServices.vva.UseGlobalValues = true;
                    //        public int Contracto { get; set; }
                    //public double Value { get; set; }
                    c.financing = new Financing();
                    c.financing.FinancingTypeCode = 3;
                    c.financing.PaymentMethodId = 2;
                    c.financing.PaymentAfter = 30;
                    c.financing.IncludeServices = false;
                    c.financing.FinancingFactors = new FinancingFactors();
                    c.financing.FinancingFactors.Monthly = new List<Monthly>();
                    c.financing.FinancingFactors.Monthly.Add(new Monthly { Contracto = 12, Value = 0.1281742 });
                    c.financing.FinancingFactors.Monthly.Add(new Monthly { Contracto = 24, Value = 0.0570024 });
                    c.financing.FinancingFactors.Monthly.Add(new Monthly { Contracto = 36, Value = 0.0362743 });
                }
                campanhas.Add(c);
            }

            return campanhas;
        }





        [AcceptVerbs("GET", "POST")]
        [ActionName("Families")]
        public List<DI_Families> Families()
        {
            return db.BB_Margin.Select(x => new DI_Families { Family = x.Family, Margin = x.Margin }).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Clientes")]
        public List<BB_Clientes_> Clientes([FromBody]Owner_ Owner)
        {
            AspNetUsers user = usersDB.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            return db.BB_Clientes.Where(x => x.Owner == user.DisplayName).Select(x => new BB_Clientes_ { accountnumber = x.accountnumber, Name = x.Name, NIF = x.NIF, Owner = x.Owner, PostalCode = x.PostalCode, address1_line1 = x.address1_line1, emailaddress1 = x.emailaddress1, Segment = x.Segment, GMA = x.GMA, Holding = x.Holding, Blocked = x.Blocked }).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Clientes_DSO")]
        public List<BB_Clientes_> Clientes_DSO([FromBody]Owner_ Owner)
        {
            List<BB_Clientes_> listaClientes = new List<BB_Clientes_>();
            List<AspNetUsers> listUsers = usersDB.AspNetUsers.Where(x => x.ManagerEmail == Owner.Owner).ToList();

            //Retorna os do DSO (proprio dele)
            listaClientes = db.BB_Clientes.Where(x => x.Owner == Owner.Owner).Select(x => new BB_Clientes_ { accountnumber = x.accountnumber, Name = x.Name, NIF = x.NIF, Owner = x.Owner, PostalCode = x.PostalCode, address1_line1 = x.address1_line1, emailaddress1 = x.emailaddress1, Segment = x.Segment, GMA = x.GMA, Holding = x.Holding, Blocked = x.Blocked }).ToList();

            foreach (var x in db.BB_Clientes.ToList())
            {

                foreach (var u in listUsers)
                {
                    try
                    {
                        if (x.Owner == u.DisplayName)
                            listaClientes.Add(new BB_Clientes_ { accountnumber = x.accountnumber, Name = x.Name, NIF = x.NIF, Owner = x.Owner, PostalCode = x.PostalCode, Blocked = x.Blocked, address1_line1 = x.address1_line1, emailaddress1 = x.emailaddress1, Segment = x.Segment, GMA = x.GMA, Holding = x.Holding });
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }

            return listaClientes;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ClientesAll")]
        public List<BB_Clientes_> ClientesAll()
        {
            return db.BB_Clientes.Select(x => new BB_Clientes_ { accountnumber = x.accountnumber, Name = x.Name, NIF = x.NIF, Owner = x.Owner, Segment = x.Segment, GMA = x.GMA, Holding = x.Holding, Blocked = x.Blocked }).ToList();
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("Equipamentos")]
        public List<BB_Equipamentos> Equipamentos()
        {
            return db.BB_Equipamentos.ToList();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_LD_TipoContrato")]
        public List<LD_TipoContrato> Get_LD_TipoContrato()
        {
            return dbLeaseDesk.LD_TipoContrato.Where(x=>x.Enable ==true).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("BNPFactors")]
        public List<BB_Bank_Bnp> BNPFactors()
        {
            return db.BB_Bank_Bnp.ToList();
        }




        [AcceptVerbs("GET", "POST")]
        [ActionName("Commissions")]
        public List<BB_Commission> Commissions()
        {
            return db.BB_Commission.ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetOPSImplementPacks")]
        public List<BB_OPS_Implement_Packs> GetOPSImplementPacks()
        {
            return db.BB_OPS_Implement_Packs.Where(x => x.InCatalog == true).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetOPSManagePacks")]
        public List<BB_OPS_Manage_Packs> GetOPSManagePacks()
        {
            return db.BB_OPS_Manage_Packs.Where(x => x.InCatalog == true).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetServiceOPS")]
        public List<DIItem> GetServiceOPS()
        {
            return db.BB_Data_Integration.Where(x =>
                    x.CodeRef == "9969PT00176" ||
                    x.CodeRef == "9969PT00177" ||
                    x.CodeRef == "9969PT00178" ||
                    x.CodeRef == "9969PT00179" ||
                    x.CodeRef == "9969PT00180" ||
                    x.CodeRef == "9969PT00181" ||
                    x.CodeRef == "9969PT00182" ||
                    x.CodeRef == "9969PT00183" ||
                    x.CodeRef == "9969PT00184" ||
                    x.CodeRef == "9969PT00185" ||
                    x.CodeRef == "9969PT00186" ||
                    x.CodeRef == "9969PT00187" ||
                    //x.CodeRef == "9969PT00173" ||
                    //x.CodeRef == "9969PT00143" ||
                    //x.CodeRef == "9969PT00171" ||
                    //x.CodeRef == "9969PT00172" ||
                    //x.CodeRef == "9969PT00201" ||
                    //x.CodeRef == "9969PT00202" ||
                    //x.CodeRef == "9969PT00203" ||
                    x.CodeRef == "9960OD32006" ||
                    x.CodeRef == "9960OD23150" ||
                    x.CodeRef == "OPD_ECO_INST" ||
                    x.CodeRef == "9969PT00188" ||
                    x.CodeRef == "9969PT00189" ||
                    x.CodeRef == "9969PT00190" ||
                    x.CodeRef == "9969PT00191" ||
                    x.CodeRef == "9969PT00192" ||
                    x.CodeRef == "9969PT00193" ||
                    x.CodeRef == "9969PT00194" ||
                    x.CodeRef == "9969PT00195" ||
                    x.CodeRef == "9969PT00196" ||
                    x.CodeRef == "9969PT00197" ||
                    x.CodeRef == "9969PT00198" ||
                    x.CodeRef == "9969PT00199" ||
                    x.CodeRef == "9969PT00200" ||
                    //x.CodeRef == "9969PT00204" ||
                    //x.CodeRef == "9969PT00205" ||
                    //x.CodeRef == "9969PT00206" ||
                    x.CodeRef == "996919-SP00048A")
            .Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetRecolhaToners")]
        public List<DIItem> GetRecolhaToners()
        {
            return db.BB_Data_Integration.Where(x => x.ID == 2251 || x.ID == 2252 || x.ID == 2253).Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("DuplicateProposal")]
        public HttpResponseMessage DuplicateProposal(LoadProposalInfo p)
        {
            ActionResponse err = new ActionResponse();
            err = DuplicateDraft(p);
            //switch (p.Status)
            //{
            //    case "DRAFT":
            //        err = DuplicateDraft(p);
            //        break;
            //    default:
            //        break;

            //}


            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err); ;
        }

        private ActionResponse DuplicateDraft(LoadProposalInfo p)
        {
            ActionResponse err = new ActionResponse();

            var newP = db.BB_Proposal.Find(p.ProposalId);

            //newP.Status = 1;
            newP.CreatedBy = "Login";
            newP.ModifiedBy = "UserBatatas";
            newP.CreatedTime = DateTime.Now;
            newP.ModifiedTime = DateTime.Now;
            try
            {
                db.BB_Proposal.Add(newP);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            ProposalBLL p1 = new ProposalBLL();
            ActionResponse err1 = p1.LoadProposal(new LoadProposalInfo() { ProposalId = newP.ID });

            ProposalRootObject prp = new ProposalRootObject();
            prp.Draft = err1.ProposalObj.Draft;
            prp.Summary = err1.ProposalObj.Summary;
            ActionResponse err12 = p1.ProposalDraftSaveAs(prp);


            return err12;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Nodes1")]
        public List<JsTreeModel> Nodes1()
        {
            var nodes = new List<JsTreeModel>();
            try
            {
                List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();
                List<String> familias = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS").Select(x => x.Family).Distinct().ToList();

                foreach (var familia in familias)
                {
                    JsTreeModel familiaJSTREE = new JsTreeModel();
                    familiaJSTREE.id = familia;
                    familiaJSTREE.text = familia;
                    familiaJSTREE.parent = "#";
                    familiaJSTREE.a_attr = "";
                    nodes.Add(familiaJSTREE);
                }

                List<String> familias1 = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family != "OPSHW").Select(x => x.Family).Distinct().ToList();
                foreach (var familia in familias1)
                {
                    var itemsFamilia = db.BB_Data_Integration.Where(x => x.Family == familia).ToList();
                    foreach (var objFamilia in itemsFamilia)
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = objFamilia.CodeRef;
                        itemfamilia.text = objFamilia.Description_English;
                        itemfamilia.parent = familia;
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = true;
                        nodes.Add(itemfamilia);
                    }
                }

                //WORKSHEET
                foreach (var w in db.BB_WorkSheet_Metadata)
                {
                    JsTreeModel workesheet1 = new JsTreeModel();
                    workesheet1.id = w.WorkSheet;
                    workesheet1.text = w.WorkSheet;
                    workesheet1.parent = "OPSHW";
                    nodes.Add(workesheet1);
                }

                //EQUIPAMENTOS
                foreach (var e in db.BB_Equipamentos)
                {
                    JsTreeModel equipamento = new JsTreeModel();
                    equipamento.id = e.CodeRef;
                    equipamento.text = e.Name;
                    equipamento.parent = db.BB_WorkSheet_Metadata.Where(x => x.ID == e.WorkSheet_MetadataID).FirstOrDefault().WorkSheet.ToString();
                    equipamento.icon = "jstree-file";
                    equipamento.selectable = true;
                    nodes.Add(equipamento);

                    foreach (var str in categoriasDistinct)
                    {
                        if (str != null)
                        {
                            JsTreeModel categorias = new JsTreeModel();
                            categorias.id = e.CodeRef + "_" + str;
                            categorias.text = str;
                            categorias.parent = e.CodeRef;
                            nodes.Add(categorias);

                            //TONERS
                            var consumiveis = (from m in db.BB_WorkSheet_Metadata
                                               join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                               join ec in db.BB_REL_Equipament_Consumable on eq.ID equals ec.ID_Equipament
                                               join c in db.BB_Consumables on ec.ID_Consumables equals c.ID
                                               join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                               where (e.CodeRef == eq.CodeRef && str == di.Category_PT)
                                               select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                               .ToList();

                            foreach (var c in consumiveis)
                            {
                                JsTreeModel consumivel = new JsTreeModel();
                                consumivel.id = e.Name + "_" + c.Name;
                                consumivel.text = c.Name;
                                consumivel.parent = e.CodeRef + "_" + str;
                                consumivel.icon = "jstree-file";
                                consumivel.selectable = true;
                                nodes.Add(consumivel);
                            }

                            //Acessorios
                            var acessorios = (from m in db.BB_WorkSheet_Metadata
                                              join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                              join ec in db.BB_REL_Equipament_Acessorie on eq.ID equals ec.ID_Equipament
                                              join c in db.BB_Acessories on ec.ID_Acessorie equals c.ID
                                              join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                              where (e.CodeRef == eq.CodeRef && str == di.Category_PT)
                                              select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                               .ToList();

                            foreach (var c in acessorios)
                            {
                                JsTreeModel consumivel = new JsTreeModel();
                                consumivel.id = c.CodRef;
                                consumivel.text = c.Name;
                                consumivel.parent = e.CodeRef + "_" + str;
                                consumivel.icon = "jstree-file";
                                consumivel.a_attr = c.Description_Portuguese;
                                consumivel.selectable = true;
                                nodes.Add(consumivel);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.InnerException.ToString();

            }
            return nodes;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Nodes2")]
        public List<JsTreeModel> Nodes2()
        {
            var nodes = new List<JsTreeModel>();
            try
            {
                List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();
                List<String> familias = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS").Select(x => x.Family).Distinct().ToList();

                foreach (var familia in familias)
                {
                    JsTreeModel familiaJSTREE = new JsTreeModel();
                    familiaJSTREE.id = familia;
                    familiaJSTREE.text = familia;
                    familiaJSTREE.parent = "#";
                    familiaJSTREE.a_attr = "";
                    nodes.Add(familiaJSTREE);
                }

                List<DIItem> dataintegration = db.BB_Data_Integration.Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();

                foreach (var item in dataintegration)
                {
                    var isEquipamento = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                    if (isEquipamento != null)
                    {
                        var nameWorksheet = db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).Select(x => x.WorkSheet).FirstOrDefault();
                        //WORKSHEET
                        var n = nodes.Where(x => x.id == nameWorksheet).FirstOrDefault();


                        if (n == null)
                        {
                            var worksheet = db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).FirstOrDefault();
                            JsTreeModel workesheet1 = new JsTreeModel();
                            workesheet1.id = worksheet.WorkSheet;
                            workesheet1.text = worksheet.WorkSheet;
                            workesheet1.parent = isEquipamento.Family;
                            nodes.Add(workesheet1);
                        }



                        //EQUIPAMENTOS


                        JsTreeModel equipamento = new JsTreeModel();
                        equipamento.id = isEquipamento.CodeRef;
                        equipamento.text = isEquipamento.Name;
                        equipamento.parent = db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).FirstOrDefault().WorkSheet.ToString();
                        equipamento.icon = "jstree-file";
                        equipamento.selectable = true;
                        nodes.Add(equipamento);

                        foreach (var str in categoriasDistinct)
                        {
                            if (str != null)
                            {
                                JsTreeModel categorias = new JsTreeModel();
                                categorias.id = isEquipamento.CodeRef + "_" + str;
                                categorias.text = str;
                                categorias.parent = isEquipamento.CodeRef;
                                nodes.Add(categorias);

                                //TONERS
                                var consumiveis = (from m in db.BB_WorkSheet_Metadata
                                                   join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                                   join ec in db.BB_REL_Equipament_Consumable on eq.ID equals ec.ID_Equipament
                                                   join c in db.BB_Consumables on ec.ID_Consumables equals c.ID
                                                   join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                                   where (isEquipamento.CodeRef == eq.CodeRef && str == di.Category_PT)
                                                   select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                                   .ToList();

                                foreach (var c in consumiveis)
                                {
                                    JsTreeModel consumivel = new JsTreeModel();
                                    consumivel.id = isEquipamento.Name + "_" + c.Name;
                                    consumivel.text = c.Name;
                                    consumivel.parent = isEquipamento.CodeRef + "_" + str;
                                    consumivel.icon = "jstree-file";
                                    consumivel.selectable = true;
                                    nodes.Add(consumivel);
                                }

                                //Acessorios
                                var acessorios = (from m in db.BB_WorkSheet_Metadata
                                                  join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                                  join ec in db.BB_REL_Equipament_Acessorie on eq.ID equals ec.ID_Equipament
                                                  join c in db.BB_Acessories on ec.ID_Acessorie equals c.ID
                                                  join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                                  where (isEquipamento.CodeRef == eq.CodeRef && str == di.Category_PT)
                                                  select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                                   .ToList();

                                foreach (var c in acessorios)
                                {
                                    JsTreeModel consumivel = new JsTreeModel();
                                    consumivel.id = c.CodRef;
                                    consumivel.text = c.Name;
                                    consumivel.parent = isEquipamento.CodeRef + "_" + str;
                                    consumivel.icon = "jstree-file";
                                    consumivel.a_attr = c.Description_Portuguese;
                                    consumivel.selectable = true;
                                    nodes.Add(consumivel);
                                }

                            }
                        }
                    }
                    else
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item.CodeRef;
                        itemfamilia.text = item.Description;
                        itemfamilia.parent = item.Family;
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = true;
                        nodes.Add(itemfamilia);
                    }


                }

                //    foreach (var objFamilia in itemsFamilia)
                //    {
                //        var isEquipment = db.BB_Equipamentos.Where(x => x.CodeRef == objFamilia.CodeRef).FirstOrDefault();

                //        if (isEquipment != null)
                //        {
                //            JsTreeModel workesheet1 = new JsTreeModel();
                //            //workesheet
                //        }


                //        JsTreeModel itemfamilia = new JsTreeModel();
                //        itemfamilia.id = objFamilia.CodeRef;
                //        itemfamilia.text = objFamilia.Description_English;
                //        itemfamilia.parent = familia;
                //        itemfamilia.a_attr = "";
                //        itemfamilia.icon = "jstree-file";
                //        itemfamilia.selectable = true;
                //        nodes.Add(itemfamilia);
                //    }
                //}

                ////WORKSHEET
                //foreach (var w in db.BB_WorkSheet_Metadata)
                //{
                //    JsTreeModel workesheet1 = new JsTreeModel();
                //    workesheet1.id = w.WorkSheet;
                //    workesheet1.text = w.WorkSheet;
                //    workesheet1.parent = "OPSHW";
                //    nodes.Add(workesheet1);
                //}

                ////EQUIPAMENTOS
                //foreach (var e in db.BB_Equipamentos)
                //{

                //    JsTreeModel equipamento = new JsTreeModel();
                //    equipamento.id = e.CodeRef;
                //    equipamento.text = e.Name;
                //    equipamento.parent = db.BB_WorkSheet_Metadata.Where(x => x.ID == e.WorkSheet_MetadataID).FirstOrDefault().WorkSheet.ToString();
                //    equipamento.icon = "jstree-file";
                //    equipamento.selectable = true;
                //    nodes.Add(equipamento);

                //    foreach (var str in categoriasDistinct)
                //    {
                //        if (str != null)
                //        {
                //            JsTreeModel categorias = new JsTreeModel();
                //            categorias.id = e.CodeRef + "_" + str;
                //            categorias.text = str;
                //            categorias.parent = e.CodeRef;
                //            nodes.Add(categorias);

                //            //TONERS
                //            var consumiveis = (from m in db.BB_WorkSheet_Metadata
                //                               join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                //                               join ec in db.BB_REL_Equipament_Consumable on eq.ID equals ec.ID_Equipament
                //                               join c in db.BB_Consumables on ec.ID_Consumables equals c.ID
                //                               join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                //                               where (e.CodeRef == eq.CodeRef && str == di.Category_PT)
                //                               select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                //                               .ToList();

                //            foreach (var c in consumiveis)
                //            {
                //                JsTreeModel consumivel = new JsTreeModel();
                //                consumivel.id = e.Name + "_" + c.Name;
                //                consumivel.text = c.Name;
                //                consumivel.parent = e.CodeRef + "_" + str;
                //                consumivel.icon = "jstree-file";
                //                consumivel.selectable = true;
                //                nodes.Add(consumivel);
                //            }

                //            //Acessorios
                //            var acessorios = (from m in db.BB_WorkSheet_Metadata
                //                              join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                //                              join ec in db.BB_REL_Equipament_Acessorie on eq.ID equals ec.ID_Equipament
                //                              join c in db.BB_Acessories on ec.ID_Acessorie equals c.ID
                //                              join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                //                              where (e.CodeRef == eq.CodeRef && str == di.Category_PT)
                //                              select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                //                               .ToList();

                //            foreach (var c in acessorios)
                //            {
                //                JsTreeModel consumivel = new JsTreeModel();
                //                consumivel.id = c.CodRef;
                //                consumivel.text = c.Name;
                //                consumivel.parent = e.CodeRef + "_" + str;
                //                consumivel.icon = "jstree-file";
                //                consumivel.a_attr = c.Description_Portuguese;
                //                consumivel.selectable = true;
                //                nodes.Add(consumivel);
                //            }

                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                ex.InnerException.ToString();

            }
            return nodes;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("NodesJstree")]
        public JsTreeModelFamilie NodesJstree()
        {
            var nodes = new JsTreeModelFamilie();
            nodes.HW = new List<JsTreeModel>();
            nodes.SV = new List<JsTreeModel>();
            nodes.SW = new List<JsTreeModel>();

            //List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();

            List<String> familiasHW = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains("HW")).Select(x => x.Family).Distinct().ToList();
            List<DIItem> dataintegrationHW = db.BB_Data_Integration.Where(x => x.Family.Contains("HW") && x.Type == "A").Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();
            nodes.HW = PopulateFamily_v1(nodes.HW, "HW", familiasHW, dataintegrationHW);

            List<String> familiasSV = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains("SV") && x.Type == "A").Select(x => x.Family).Distinct().ToList();
            List<DIItem> dataintegrationSV = db.BB_Data_Integration.Where(x => x.Family.Contains("SV") && x.Type == "A").Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();
            nodes.SV = PopulateFamily_v2(nodes.SV, "SV", familiasSV, dataintegrationSV);

            List<DIItem> dataintegrationSW = db.BB_Data_Integration.Where(x => x.Family.Contains("SW") && x.Type == "A").Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();
            List<String> familiasSW = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains("SW")).Select(x => x.Family).Distinct().ToList();
            nodes.SW = PopulateFamily_v2(nodes.SW, "SW", familiasSW, dataintegrationSW);


            return nodes;
        }

        private List<JsTreeModel> PopulateFamily(List<JsTreeModel> nodes, string strFamilia, List<string> categoriasDistinct, List<String> familias, List<DIItem> dataintegration)
        {

            try
            {
                //List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();

                //List<String> familias = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains(strFamilia)).Select(x => x.Family).Distinct().ToList();

                foreach (var familia in familias)
                {
                    JsTreeModel familiaJSTREE = new JsTreeModel();
                    familiaJSTREE.id = familia;
                    familiaJSTREE.text = familia;
                    familiaJSTREE.parent = "#";
                    familiaJSTREE.a_attr = "";
                    nodes.Add(familiaJSTREE);
                }

                //List<DIItem> dataintegration = db.BB_Data_Integration.Where(x => x.Family.Contains(strFamilia)).Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();

                foreach (var item in dataintegration)
                {
                    var isEquipamento = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                    if (isEquipamento != null)
                    {
                        var nameWorksheet = db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).Select(x => x.WorkSheet).FirstOrDefault();
                        //WORKSHEET
                        var n = nodes.Where(x => x.id == nameWorksheet).FirstOrDefault();

                        var worksheet = db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).FirstOrDefault();
                        if (n == null && isEquipamento.Family != null && isEquipamento.Family != "")
                        {

                            JsTreeModel workesheet1 = new JsTreeModel();
                            workesheet1.id = worksheet.WorkSheet;
                            workesheet1.text = worksheet.WorkSheet;
                            workesheet1.parent = isEquipamento.Family;
                            nodes.Add(workesheet1);
                        }

                        //EQUIPAMENTOS
                        JsTreeModel equipamento = new JsTreeModel();
                        equipamento.id = isEquipamento.CodeRef;
                        equipamento.text = isEquipamento.Name;
                        equipamento.parent = worksheet.WorkSheet; //db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).FirstOrDefault().WorkSheet.ToString();
                        equipamento.icon = "jstree-file";
                        equipamento.selectable = true;
                        equipamento.codeRef = isEquipamento.CodeRef;
                        equipamento.data = "selectable";
                        equipamento.li_attr = isEquipamento.CodeRef;
                        nodes.Add(equipamento);

                        foreach (var str in categoriasDistinct)
                        {
                            if (str != null)
                            {
                                JsTreeModel categorias = new JsTreeModel();
                                categorias.id = isEquipamento.CodeRef + "_" + str + nameWorksheet;
                                categorias.text = str;
                                categorias.parent = isEquipamento.CodeRef;

                                nodes.Add(categorias);

                                //TONERS
                                var consumiveis = (from m in db.BB_WorkSheet_Metadata
                                                   join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                                   join ec in db.BB_REL_Equipament_Consumable on eq.ID equals ec.ID_Equipament
                                                   join c in db.BB_Consumables on ec.ID_Consumables equals c.ID
                                                   join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                                   where (isEquipamento.Family != null && isEquipamento.Family != "" && isEquipamento.CodeRef == eq.CodeRef && str == di.Category_PT)
                                                   select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                                   .ToList();

                                foreach (var c in consumiveis)
                                {
                                    JsTreeModel consumivel = new JsTreeModel();
                                    consumivel.id = c.CodRef + equipamento.id;
                                    consumivel.text = c.Name;
                                    consumivel.parent = isEquipamento.CodeRef + "_" + str + nameWorksheet;
                                    consumivel.icon = "jstree-file";
                                    consumivel.selectable = true;
                                    consumivel.li_attr = c.CodRef;
                                    consumivel.codeRef = c.CodRef;
                                    consumivel.data = "selectable";
                                    nodes.Add(consumivel);
                                }

                                //Acessorios
                                var acessorios = (from m in db.BB_WorkSheet_Metadata
                                                  join eq in db.BB_Equipamentos on m.ID equals eq.WorkSheet_MetadataID
                                                  join ec in db.BB_REL_Equipament_Acessorie on eq.ID equals ec.ID_Equipament
                                                  join c in db.BB_Acessories on ec.ID_Acessorie equals c.ID
                                                  join di in db.BB_Data_Integration on c.CodRef equals di.CodeRef
                                                  where (isEquipamento.Family != null && isEquipamento.Family != "" && isEquipamento.CodeRef == eq.CodeRef && str == di.Category_PT)
                                                  select (new { c.CodRef, c.Name, di.Description_Portuguese })).Distinct()
                                                   .ToList();

                                foreach (var c in acessorios)
                                {
                                    JsTreeModel consumivel = new JsTreeModel();
                                    consumivel.id = c.CodRef + equipamento.id;
                                    consumivel.text = c.Name;
                                    consumivel.parent = isEquipamento.CodeRef + "_" + str + nameWorksheet;
                                    consumivel.icon = "jstree-file";
                                    consumivel.a_attr = c.Description_Portuguese;
                                    consumivel.selectable = true;
                                    consumivel.codeRef = c.CodRef;
                                    consumivel.li_attr = c.CodRef;
                                    consumivel.data = "selectable";
                                    nodes.Add(consumivel);
                                }

                            }
                        }
                    }
                    //item.Family != "OPSHW"
                    if (item.Family != "")
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item.CodeRef;
                        itemfamilia.text = item.Description;
                        itemfamilia.parent = item.Family;
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = true;
                        itemfamilia.codeRef = item.CodeRef;
                        itemfamilia.data = "selectable";
                        itemfamilia.li_attr = item.CodeRef;
                        nodes.Add(itemfamilia);
                    }


                }
            }
            catch (Exception ex)
            {
                ex.InnerException.ToString();

            }
            return nodes;
        }

        private List<JsTreeModel> PopulateFamily_v1(List<JsTreeModel> nodes, string strFamilia, List<String> familias, List<DIItem> dataintegration)
        {

            try
            {
                //List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();

                //List<String> familias = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains(strFamilia)).Select(x => x.Family).Distinct().ToList();

                foreach (var familia in familias)
                {
                    JsTreeModel familiaJSTREE = new JsTreeModel();
                    familiaJSTREE.id = familia;
                    familiaJSTREE.text = familia;
                    familiaJSTREE.parent = "#";
                    familiaJSTREE.a_attr = "";
                    nodes.Add(familiaJSTREE);
                }

                //List<DIItem> dataintegration = db.BB_Data_Integration.Where(x => x.Family.Contains(strFamilia)).Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();

                foreach (var item in dataintegration)
                {
                    //item.Family != "OPSHW"
                    if (item.Family != "OPSHW")
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item.Family + "_" + item.CodeRef;
                        itemfamilia.text = item.Description;
                        itemfamilia.parent = item.Family;
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = true;
                        itemfamilia.codeRef = item.CodeRef;
                        itemfamilia.data = "selectable";
                        itemfamilia.li_attr = item.CodeRef;
                        nodes.Add(itemfamilia);
                    }

                }


                List<string> lstWorksheets = db.BB_Machines_Compatibility.Select(x => x.WorkSheet).Distinct().ToList();

                foreach (var item in lstWorksheets)
                {
                    if (item != "AccurioPrint C759" &&
                        item != "Accurio Print C3070L" &&
                        item != "AccurioPress C3070_80(P)-C83hc" &&
                        item != "Accurio PRESS C6085-C6100" &&
                         item != "Bizhub PRO 958" &&
                         item != "Bizhub PRO 1100" &&
                         item != "AccurioPress 6120_6136_6136P" &&
                          item != "Bizhub PRESS 2250" &&
                          item != "KIP 7970" &&
                            item != "KIP 7170" &&
                             item != "Accurio Label 230" &&
                              item != "Meteor Unlimited Colors" &&
                        item != "MGI Jet Varnish 3DS  " &&
                         item != "MGI Jet Varnish 3D" &&
                        item != "MGI Jet Varnish 3D EVO" &&
                         item != "MGI Jet Varnish 3D WEB")
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item;
                        itemfamilia.text = item;
                        itemfamilia.parent = "OPSHW";
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = false;
                        itemfamilia.codeRef = item;
                        itemfamilia.data = "";
                        itemfamilia.li_attr = item;
                        nodes.Add(itemfamilia);

                        List<BB_Machines_Compatibility> grupoMaquinas = db.BB_Machines_Compatibility.Where(x => x.WorkSheet == item).ToList();

                        List<String> listaCategorias = grupoMaquinas.Select(x => x.Type).Distinct().ToList();
                        foreach (var categoria in listaCategorias)
                        {

                            itemfamilia = new JsTreeModel();
                            itemfamilia.id = item + categoria;
                            itemfamilia.text = categoria;
                            itemfamilia.parent = item;
                            itemfamilia.a_attr = "";
                            itemfamilia.icon = "jstree-file";
                            itemfamilia.selectable = true;
                            itemfamilia.codeRef = categoria;
                            itemfamilia.data = "";
                            itemfamilia.li_attr = categoria;
                            nodes.Add(itemfamilia);

                            List<BB_Machines_Compatibility> listMaquina = grupoMaquinas.Where(x => x.Type == categoria).ToList();

                            foreach (var maquina in listMaquina)
                            {
                                if (maquina.Family != "")
                                {
                                    itemfamilia = new JsTreeModel();
                                    itemfamilia.id = maquina.Codigo + item + categoria;
                                    itemfamilia.text = maquina.Descricao;
                                    itemfamilia.parent = item + categoria;
                                    itemfamilia.a_attr = "";
                                    itemfamilia.icon = "jstree-file";
                                    itemfamilia.selectable = true;
                                    itemfamilia.codeRef = maquina.Codigo;
                                    itemfamilia.data = "selectable";
                                    itemfamilia.li_attr = maquina.Codigo;
                                    nodes.Add(itemfamilia);
                                }
                            }
                        }
                    }
                    else
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item;
                        itemfamilia.text = item;
                        itemfamilia.parent = "PPHW";
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = false;
                        itemfamilia.codeRef = item;
                        itemfamilia.data = "";
                        itemfamilia.li_attr = item;
                        nodes.Add(itemfamilia);

                        List<BB_Machines_Compatibility> grupoMaquinas = db.BB_Machines_Compatibility.Where(x => x.WorkSheet == item).ToList();

                        List<String> listaCategorias = grupoMaquinas.Select(x => x.Type).Distinct().ToList();
                        foreach (var categoria in listaCategorias)
                        {

                            itemfamilia = new JsTreeModel();
                            itemfamilia.id = item + categoria;
                            itemfamilia.text = categoria;
                            itemfamilia.parent = item;
                            itemfamilia.a_attr = "";
                            itemfamilia.icon = "jstree-file";
                            itemfamilia.selectable = true;
                            itemfamilia.codeRef = categoria;
                            itemfamilia.data = "";
                            itemfamilia.li_attr = categoria;
                            nodes.Add(itemfamilia);

                            List<BB_Machines_Compatibility> listMaquina = grupoMaquinas.Where(x => x.Type == categoria).ToList();

                            foreach (var maquina in listMaquina)
                            {
                                if (maquina.Family != "")
                                {
                                    itemfamilia = new JsTreeModel();
                                    itemfamilia.id = maquina.Codigo + item + categoria;
                                    itemfamilia.text = maquina.Descricao;
                                    itemfamilia.parent = item + categoria;
                                    itemfamilia.a_attr = "";
                                    itemfamilia.icon = "jstree-file";
                                    itemfamilia.selectable = true;
                                    itemfamilia.codeRef = maquina.Codigo;
                                    itemfamilia.data = "selectable";
                                    itemfamilia.li_attr = maquina.Codigo;
                                    nodes.Add(itemfamilia);
                                }
                            }
                        }
                    }
                }

                //JsTreeModel equipamento = new JsTreeModel();
                //equipamento.id = isEquipamento.CodeRef;
                //equipamento.text = isEquipamento.Name;
                //equipamento.parent = worksheet.WorkSheet; //db.BB_WorkSheet_Metadata.Where(x => x.ID == isEquipamento.WorkSheet_MetadataID).FirstOrDefault().WorkSheet.ToString();
                //equipamento.icon = "jstree-file";
                //equipamento.selectable = true;
                //equipamento.codeRef = isEquipamento.CodeRef;
                //equipamento.data = "selectable";
                //equipamento.li_attr = isEquipamento.CodeRef;
                //nodes.Add(equipamento);


            }
            catch (Exception ex)
            {
                ex.InnerException.ToString();

            }
            return nodes;
        }

        private List<JsTreeModel> PopulateFamily_v2(List<JsTreeModel> nodes, string strFamilia, List<String> familias, List<DIItem> dataintegration)
        {

            try
            {
                //List<string> categoriasDistinct = db.BB_Data_Integration.Select(x => x.Category_PT).Distinct().ToList();

                //List<String> familias = db.BB_Data_Integration.Where(x => x.Family != "#N/A" && x.Family != "0" && x.Family != "" && x.Family != "CS" && x.Family.Contains(strFamilia)).Select(x => x.Family).Distinct().ToList();

                foreach (var familia in familias)
                {
                    JsTreeModel familiaJSTREE = new JsTreeModel();
                    familiaJSTREE.id = familia;
                    familiaJSTREE.text = familia;
                    familiaJSTREE.parent = "#";
                    familiaJSTREE.a_attr = "";
                    nodes.Add(familiaJSTREE);
                }

                //List<DIItem> dataintegration = db.BB_Data_Integration.Where(x => x.Family.Contains(strFamilia)).Select(x => new DIItem { CodeRef = x.CodeRef, Description = x.Description_English, Family = x.Family, Name = x.Description_English }).ToList();

                foreach (var item in dataintegration)
                {
                    //item.Family != "OPSHW"
                    if (item.Family != "OPSHW")
                    {
                        JsTreeModel itemfamilia = new JsTreeModel();
                        itemfamilia.id = item.Family + "_" + item.CodeRef;
                        itemfamilia.text = item.Description;
                        itemfamilia.parent = item.Family;
                        itemfamilia.a_attr = "";
                        itemfamilia.icon = "jstree-file";
                        itemfamilia.selectable = true;
                        itemfamilia.codeRef = item.CodeRef;
                        itemfamilia.data = "selectable";
                        itemfamilia.li_attr = item.CodeRef;
                        nodes.Add(itemfamilia);
                    }

                }

            }
            catch (Exception ex)
            {
                ex.InnerException.ToString();

            }
            return nodes;
        }
        public FinancingData GetFinancingData()
        {
            FinancingData f = new FinancingData();

            f.FinancingContractType = db.BB_FinancingContractType.ToList();
            f.FinancingPaymentMethod = db.BB_FinancingPaymentMethod.ToList();
            f.FinancingType = db.BB_FinancingType.ToList();

            return f;

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductUnits")]
        public List<BB_Product_Unit> GetProductUnits()
        {
            List<BB_Product_Unit> productUnits = db.BB_Product_Unit.ToList();
            return productUnits;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductGroups")]
        public List<BB_Product_Group> GetProductGroups()
        {
            List<BB_Product_Group> productGroups = db.BB_Product_Group.ToList();
            return productGroups;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductTypes")]
        public List<BB_Product_Type> GetProductTypes()
        {
            List<BB_Product_Type> productTypes = db.BB_Product_Type.ToList();
            return productTypes;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductSubTypes")]
        public List<BB_Product_SubType> GetProductSubTypes()
        {
            List<BB_Product_SubType> productSubTypes = db.BB_Product_SubType.ToList();
            return productSubTypes;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductCategories")]
        public List<BB_Product_Category> GetProductCategories()
        {
            List<BB_Product_Category> productCategories = db.BB_Product_Category.ToList();
            return productCategories;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProductSubCategories")]
        public List<BB_Product_SubCategory> GetProductSubCategories()
        {
            List<BB_Product_SubCategory> productSubCategories = db.BB_Product_SubCategory.ToList();
            return productSubCategories;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProducts")]
        public List<BB_Product> GetProducts()
        {
            List<BB_Product> product = db.BB_Product.ToList();
            return product;
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("RoutineEmail")]
        public IHttpActionResult RoutineEmail()
        {
           
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("RoutineEmail" + "-Start", EventLogEntryType.Information, 101, 1);
                }
                

                string fileName = @"C:\DeployPROD\23122020\Debug\ConsoleAppEmail.exe";
                Process.Start(fileName);

            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("RoutineEmail" + ex.Message, EventLogEntryType.Information, 101, 1);
                }
            }
            return Ok();
        }

        public bool RedirectionCallback(string url)
        {
            // Return true if the URL is an HTTPS URL.
            return url.ToLower().StartsWith("https://");
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAddressAcronyms")]
        public List<string> GetAddressAcronyms()
        {
            List<string> addressAcronyms = db.RD_AddressAcronyms.Select(x => x.SA).ToList();
            return addressAcronyms;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDataIntegrationByPage")]
        public List<DIItem> GetDataIntegrationByPage(int begin, int end)
        {
            // return db.BB_Data_Integration.Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();
            if (begin == 0)
            {
                return db.BB_Data_Integration.Take(10).Select(x => new DIItem { CodeRef = x.CodeRef, Family = x.Family, Name = x.Description_English, Description = x.Description_Portuguese, PVP = x.PVP, BinaryImage = x.BinaryImage, IsMarginBEU = x.IsMarginBEU, MarginBEU = x.MarginBEU }).ToList();

            }
            else
            {
                return db.BB_Data_Integration
                .Skip(begin) // Skip the first 10 records
                .Take(end) // Take the next 10 records
                .Select(x => new DIItem
                {
                    CodeRef = x.CodeRef,
                    Family = x.Family,
                    Name = x.Description_English,
                    Description = x.Description_Portuguese,
                    PVP = x.PVP,
                    BinaryImage = x.BinaryImage,
                    IsMarginBEU = x.IsMarginBEU,
                    MarginBEU = x.MarginBEU
                })
                .ToList();
            }
        }
    }


    public class FinancingData
    {
        public List<BB_FinancingContractType> FinancingContractType { get; set; }

        public List<BB_FinancingPaymentMethod> FinancingPaymentMethod { get; set; }
        public List<BB_FinancingType> FinancingType { get; set; }
    }
    public class LoadProposalInfo
    {
        public int ProposalId { get; set; }
        //public string Status { get; set; }

        //public string Login { get; set; }

    }

    public class Info
    {
        public string title { get; set; }
        public string login { get; set; }

        public int ProposalID { get; set; }
    }

    public class ActionResponse : HttpResponseMessage
    {
        //public int StatusCode { get; set; }
        public string Message { get; set; }

        public string InnerException { get; set; }

        public int ProposalIDReturn { get; set; }
        public ProposalRootObject ProposalObj { get; set; }

    }


    public class ContactosResponse : HttpResponseMessage
    {
        //public int StatusCode { get; set; }
        public List<BB_Contactos> Contactos { get; set; }

    }

    


    public class StatusActionResponse : HttpResponseMessage
    {
        //public int StatusCode { get; set; }
        public string Message { get; set; }

        public string InnerException { get; set; }

        public string State { get; set; }
        public string Status { get; set; }

    }

    public class Machine_Compatibily
    {

    }
    public class ActionFailResponse : Exception
    {

        public ActionFailResponse()
        { }
        public ActionFailResponse(string message) : base(message)
        { }

        public ActionFailResponse(string message, Exception inner)
            : base(message, inner)
        { }

        public int StatusCode { get; set; }


        public int ProposalIDReturn { get; set; }

    }

    public class ActionSucessResponse : ActionResponse
    {
        public ActionSucessResponse() { StatusCode = System.Net.HttpStatusCode.OK; }
    }

    public class DIItem
    {
        public int ID { get; set; }
        public string CodeRef { get; set; }
        public string Family { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<double> PVP { get; set; }
        public string BinaryImage { get; set; }

        public bool? IsMarginBEU { get; set; }

        public double? MarginBEU { get; set; }
        public string BOM { get; set; }
    }
    public class DI_Families
    {

        public string Family { get; set; }
        public Nullable<double> Margin { get; set; }
    }

    public class BB_BNPFactors
    {
        public string ID { get; set; }
        public string Type_Contract { get; set; }
        public string Type_Duration { get; set; }
        public float Contracto { get; set; }
        public int Volume_Start { get; set; }
        public int Volume_End { get; set; }
        public float Value { get; set; }
        public float DespesaContracto { get; set; }
        public float Iva { get; set; }
    }

    public class BB_CRM_Quotes_M
    {
        public string quotenumber { get; set; }

        public string accountnumber { get; set; }
    }

    public class JsTreeModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }

        public string codeRef { get; set; }

        public bool selectable { get; set; }
        public string data { get; set; }

        //public Equipments equipments { get; set;}
    }

    public class JsTreeModelFamilie
    {
        public List<JsTreeModel> SV { get; set; }
        public List<JsTreeModel> HW { get; set; }
        public List<JsTreeModel> SW { get; set; }

        //public Equipments equipments { get; set;}
    }




}