
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebApplication1.App_Start;
using static WebApplication1.Models.SetupXML.XSD;


namespace WebApplication1.Models.SetupXML.XML
{
    public class Deal
    {
        
        private string sftpServer = "sftp.konicaminolta.eu";
        private string sftpUser = "EnBa9ycbGg";
        private string sftpPassword = "N37H4mabKf58TrVa";
        
        //private string sftpDirectory = $"/{sftpFolder}/in"; // Ajuste conforme necessário
        public static void SerializeToXml<T>(T obj, string filePath)
        {


            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StreamWriter writer = new StreamWriter(filePath))
                {

                    serializer.Serialize(writer, obj);
                }

                Console.WriteLine($"XML file saved successfully to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while serializing to XML: {ex.Message}");
            }
        }
        private void UploadFileToSftp(string filepath)
        {
            using (var sftp = new SftpClient(sftpServer, sftpUser, sftpPassword))
            {
                string sftpFolder = @AppSettingsGet.SFTP;
                try
                {
                    sftp.Connect();

                    string sftpDirectory = $"/{sftpFolder}/in"; // Ajuste conforme necessário
                    using (var fileStream = new FileStream(filepath, FileMode.Open))
                    {
                        sftp.UploadFile(fileStream, Path.Combine(sftpDirectory, Path.GetFileName(filepath)));
                    }

                    // Verifica se o arquivo está no diretório remoto
                    var files = sftp.ListDirectory(sftpDirectory);
                    bool fileExists = false;
                    foreach (var file in files)
                    {
                        if (file.Name == Path.GetFileName(filepath))
                        {
                            fileExists = true;
                            break;
                        }
                    }

                    if (fileExists)
                    {
                        Console.WriteLine("Arquivo enviado com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("Erro: Arquivo não encontrado no diretório remoto.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao enviar o arquivo para o SFTP: {ex.Message}");
                }
                finally
                {
                    if (sftp.IsConnected)
                    {
                        sftp.Disconnect();
                    }
                }
            }
        }


        public string DealXML(int contractId)
        {
            try
            {
                //AspNetUser user = new AspNetUser();

                int? contractoID = null;
                using (var db = new BB_DB_DEVEntities2())
                {
                    string financingType = "";
                    string contractType = "";
                    string formattedDtDeal = "";

                    Random random = new Random();
                    char randomLetter = (char)('A' + random.Next(0, 26));
                    int randomNumber = random.Next(0, 9);
                    string randomNumberString = randomNumber.ToString();
                    string randomLetterNumber = randomLetter + randomNumberString;


                    List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();

                    LD_Contrato c = db.LD_Contrato.Where(x => x.ID == contractId).FirstOrDefault();
                    BB_Proposal d = db.BB_Proposal.Where(x => x.ID == c.ProposalID).FirstOrDefault();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != d.ID).FirstOrDefault();
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == d.ID).FirstOrDefault();

                    BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();
                    List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == d.ID).ToList();

                    BB_FinancingType ft = db.BB_FinancingType.Where(x => x.Code == pf.FinancingTypeCode).FirstOrDefault();

                    contractoID = c.ID;

                    switch (ft.Code)
                    {
                        case 0:
                            financingType = "K";
                            contractType = "002";
                            break;

                        case 1:
                            financingType = "L";
                            contractType = "008";
                            break;

                        case 3:
                            financingType = "AL";
                            contractType = "005";
                            break;

                        case 2:
                        case 5:
                            financingType = "M";
                            contractType = "003";
                            break;

                        default:
                            break;
                    }

                    //using (var dbuUser = new masterEntities())
                    //{
                    //    user = dbuUser.AspNetUsers.Where(x => x.Email == d.CreatedBy).FirstOrDefault();
                    //}



                    DateTime? createdTimeDealN = d.CreatedTime;
                    if (createdTimeDealN.HasValue)
                    {
                        DateTime createdTime = createdTimeDealN.Value;
                        formattedDtDeal = createdTime.ToString("yyyyMMdd");
                    }

                    //CONTRACTS
                    Contracts contractsConfig = new Contracts();
                    var collectionContracts = contractsConfig.ConfigContracts(d.ID, randomLetterNumber);

                    //ORDERS
                    List<OrdersPartners> sD_DocOrdersPartners = new List<OrdersPartners>();
                    Orders ordersConfig = new Orders();
                    var sDocOrder = new OrdersPartnersList();
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();

                    sDocOrder = ordersConfig.ConfigOrders(d.ID, randomLetterNumber, financingType);
                    foreach (var sDocPartner in sDocOrder.SdDocOrderPartner)
                    {
                        sD_DocOrdersPartners.Add(sDocPartner);
                    }
                    collectionOrders = sDocOrder.z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERs;

                    int randomNumberArckey = random.Next(100000, 999999);
                    string randomNumberArckeyString = randomNumberArckey.ToString();
                    string arckey = $"EBB_{randomNumberArckeyString}_{randomLetterNumber}";

                    //PARTERNS //Addresses //AddressesAdd
                    Partners partnersConfig = new Partners();
                    var collectionPartners = partnersConfig.ConfigPartners(d.ID, sD_DocOrdersPartners, randomLetterNumber);


                    //CONDITIONS
                    Conditions conditionsConfig = new Conditions();
                    var collectionConditions = conditionsConfig.ConfigConditions(collectionOrders, collectionContracts);

                    //Config SAP 
                    string mescod = @AppSettingsGet.SapConfigMESCOD;
                    string rcvprn = @AppSettingsGet.RCVPRN;

                    //DEAL
                    Z1ZVOE_DEAL_1 myObject = new Z1ZVOE_DEAL_1();
                    myObject.IDOC = new Z1ZVOE_DEAL_1IDOC
                    {

                        EDI_DC40 = new Z1ZVOE_DEAL_1IDOCEDI_DC40
                        {
                            TABNAM = "EDI_DC40",
                            MANDT = "100",
                            DOCREL = "700",
                            OUTMOD = "X",
                            TEST = "X",
                            IDOCTYP = "Z1ZVOE_DEAL",
                            MESTYP = "Z1ZVOE_DEAL",
                            MESCOD = mescod,   //"BES",                   //"EBB",         
                            STDMES = "Z1ZVOE",
                            SNDPOR = "eddy",
                            SNDPRT = "LS",
                            SNDPRN = "VIS",
                            RCVPRN =  rcvprn,//"EUQCLNT100", //Quando Estiver em Prod : EUPCLNT100
                            RCVPRT = "LS",
                            ARCKEY = arckey
                        },
                        Z1ZVOE_DEAL = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEAL
                        {
                            KUNAG = "1132257",//"1137222",                  //d.ClientAccountNumber
                            VKORG = "5000",
                            VTWEG = "24",
                            SPART = "01",
                            VISPN = arckey,
                            VTTYP = financingType,
                            PRREL = "X",
                            PRRDAT = formattedDtDeal,
                            BNL_RLIST = "X", //Falar com a BEU
                            SALESP = "50004700"   //   user != null && !string.IsNullOrEmpty(user.ErpNumber) ? user.ErpNumber : "",             //"50004700", //código gestor de conta , adicionar campo na tabela dos utilizadores
                         


                        },
                        Z1ZVOE_CONTRACTS = collectionContracts,
                        Z1ZVOE_ORDERS = collectionOrders,
                        Z1ZVOE_PARTNERS = collectionPartners.Partners,
                        Z1ZVOE_ADDRESSES = collectionPartners.Adresses,
                        Z1ZVOE_ADDRESSES_ADD = collectionPartners.AdressesAdd,
                        Z1ZVOE_CONDITIONS = collectionConditions,



                    };

                    string dest = AppSettingsGet.LeaseDesk_UploadFile_Contrato_DocuSign;

                    string path = dest + contractoID;
                    if (!Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);


                    string filepath = $"{path}\\{arckey}.xml";
                    SerializeToXml(myObject, filepath);
                    UploadFileToSftp(filepath);

                    return filepath;
                }
            }
            catch (Exception e)
            {
                return null;
                Console.WriteLine(e.ToString());
            }

        }

      
    }
}
