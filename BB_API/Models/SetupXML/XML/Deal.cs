﻿
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static WebApplication1.Models.SetupXML.XSD;


namespace WebApplication1.Models.SetupXML.XML
{
    public class Deal
    {
        
        private string sftpServer = "sftp.konicaminolta.eu";
        private string sftpUser = "EnBa9ycbGg";
        private string sftpPassword = "N37H4mabKf58TrVa";
        private string sftpDirectory = "/qa/in"; // Ajuste conforme necessário
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
                try
                {
                    sftp.Connect();

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


        public void DealXML(int proposalId)
        {
            try
            {
                //AspNetUser user = new AspNetUser();


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

                    BB_Proposal d = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != proposalId).FirstOrDefault();
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();
                    List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();

                    BB_FinancingType ft = db.BB_FinancingType.Where(x => x.Code == pf.FinancingTypeCode).FirstOrDefault();
                    switch (ft.Code)
                    {
                        case 0:
                            financingType = "K";
                            contractType = "002";
                            break;

                        case 1:
                        case 2:
                        case 4:
                            financingType = "L";
                            contractType = "008";
                            break;

                        case 3:
                            financingType = "AL";
                            contractType = "005";
                            break;

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
                    var collectionContracts = contractsConfig.ConfigContracts(proposalId, randomLetterNumber);

                    //ORDERS
                    List<OrdersPartners> sD_DocOrdersPartners = new List<OrdersPartners>();
                    Orders ordersConfig = new Orders();
                    var sDocOrder = new OrdersPartnersList();
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();

                    sDocOrder = ordersConfig.ConfigOrders(proposalId, randomLetterNumber, financingType);
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
                    var collectionPartners = partnersConfig.ConfigPartners(proposalId, sD_DocOrdersPartners, randomLetterNumber);


                    //CONDITIONS
                    Conditions conditionsConfig = new Conditions();
                    var collectionConditions = conditionsConfig.ConfigConditions(collectionOrders, collectionContracts);

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
                            MESCOD = "BES",                   //"EBB",         
                            STDMES = "Z1ZVOE",
                            SNDPOR = "eddy",
                            SNDPRT = "LS",
                            SNDPRN = "VIS",
                            RCVPRN = "EUQCLNT100", //Quando Estiver em Prod : EUPCLNT100
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
                    string downloadFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                    string filepath = $"{downloadFolderPath}\\{arckey}.xml";
                    SerializeToXml(myObject, filepath);
                    UploadFileToSftp(filepath);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public void WritteToCsv(Z1ZVOE_DEAL_1 myObject, int proposalId)
        {
            try
            {
                int numberOfContracts = myObject.IDOC.Z1ZVOE_CONTRACTS.Count();
                int numberOfOrders = myObject.IDOC.Z1ZVOE_ORDERS.Count();
                int numberOfPartners = myObject.IDOC.Z1ZVOE_PARTNERS.Count();
                int numberOfAddresses = myObject.IDOC.Z1ZVOE_ADDRESSES.Count();
                int numberOfAddressesAdd = myObject.IDOC.Z1ZVOE_ADDRESSES_ADD.Count();

                DateTime today = DateTime.Now;
                string formattedDate = today.ToString("dd/MM/yyyy");

                string[][] newData =
                {
                    new string[] {proposalId.ToString(), myObject.IDOC.EDI_DC40.ARCKEY, numberOfContracts.ToString(), numberOfOrders.ToString(), numberOfPartners.ToString(),
                                                                            numberOfAddresses.ToString(), numberOfAddressesAdd.ToString(), formattedDate,"" }
                };
                string filePath = "C:\\Users\\EXT0022\\OneDrive - KME\\Documents\\BB_XML_Tests.csv";

                using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    // Loop through each row of data
                    foreach (var row in newData)
                    {
                        // Write each field separated by a comma
                        sw.WriteLine(string.Join(",", row));
                    }
                }
                Console.WriteLine("CSV file updated successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}