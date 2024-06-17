using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebApplication1.Models.SetupXML
{

    public class XSD
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlRootAttribute("Z1ZVOE_DEAL_1", Namespace = "")]
        public partial class Z1ZVOE_DEAL_1
        {

            [System.Xml.Serialization.XmlElementAttribute("IDOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOC IDOC { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOC", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOC
        {

            [System.Xml.Serialization.XmlElementAttribute("EDI_DC40", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCEDI_DC40 EDI_DC40 { get; set; }

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_DEAL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEAL Z1ZVOE_DEAL { get; set; }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> _z1ZVOE_CONTRACTS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_CONTRACTS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> Z1ZVOE_CONTRACTS
            {
                get
                {
                    return this._z1ZVOE_CONTRACTS;
                }
                set
                {
                    this._z1ZVOE_CONTRACTS = value;
                }
                //private set
                //{
                //    this._z1ZVOE_CONTRACTS = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_CONTRACTSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_CONTRACTS.Count != 0);
                }
            }

            public Z1ZVOE_DEAL_1IDOC()
            {
                this._z1ZVOE_CONTRACTS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS>();
                this._z1ZVOE_ORDERS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();
                this._z1ZVOE_TEXTS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTS>();
                this._z1ZVOE_PARTNERS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS>();
                this._z1ZVOE_ADDRESSES = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES>();
                this._z1ZVOE_ADDRESSES_ADD = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD>();
                this._z1ZVOE_CONDITIONS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
                this._e1IDOCENHANCEMENT = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENT>();
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> _z1ZVOE_ORDERS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ORDERS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> Z1ZVOE_ORDERS
            {
                get
                {
                    return this._z1ZVOE_ORDERS;
                }
                set
                {
                    this._z1ZVOE_ORDERS = value;
                }
                //private set
                //{
                //    this._z1ZVOE_ORDERS = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTS> _z1ZVOE_TEXTS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_TEXTS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTS> Z1ZVOE_TEXTS
            {
                get
                {
                    return this._z1ZVOE_TEXTS;
                }
                private set
                {
                    this._z1ZVOE_TEXTS = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_TEXTSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_TEXTS.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS> _z1ZVOE_PARTNERS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_PARTNERS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS> Z1ZVOE_PARTNERS
            {
                get
                {
                    return this._z1ZVOE_PARTNERS;
                }
                set
                {
                    this._z1ZVOE_PARTNERS = value;
                }
                //private set
                //{
                //    this._z1ZVOE_PARTNERS = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_PARTNERSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_PARTNERS.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES> _z1ZVOE_ADDRESSES;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ADDRESSES", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES> Z1ZVOE_ADDRESSES
            {
                get
                {
                    return this._z1ZVOE_ADDRESSES;
                }
                set
                {
                    this._z1ZVOE_ADDRESSES = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ADDRESSESSpecified
            {
                get
                {
                    return (this.Z1ZVOE_ADDRESSES.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD> _z1ZVOE_ADDRESSES_ADD;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ADDRESSES_ADD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD> Z1ZVOE_ADDRESSES_ADD
            {
                get
                {
                    return this._z1ZVOE_ADDRESSES_ADD;
                }
                set
                {
                    this._z1ZVOE_ADDRESSES_ADD = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ADDRESSES_ADDSpecified
            {
                get
                {
                    return (this.Z1ZVOE_ADDRESSES_ADD.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS> _z1ZVOE_CONDITIONS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_CONDITIONS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS> Z1ZVOE_CONDITIONS
            {
                get
                {
                    return this._z1ZVOE_CONDITIONS;
                }
                set
                {
                    this._z1ZVOE_CONDITIONS = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_CONDITIONSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_CONDITIONS.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENT> _e1IDOCENHANCEMENT;

            [System.Xml.Serialization.XmlElementAttribute("E1IDOCENHANCEMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENT> E1IDOCENHANCEMENT
            {
                get
                {
                    return this._e1IDOCENHANCEMENT;
                }
                private set
                {
                    this._e1IDOCENHANCEMENT = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool E1IDOCENHANCEMENTSpecified
            {
                get
                {
                    return (this.E1IDOCENHANCEMENT.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute("BEGIN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCBEGIN BEGIN { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCEDI_DC40", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCEDI_DC40
        {

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private string _tABNAM = "EDI_DC40";

            [System.Xml.Serialization.XmlElementAttribute("TABNAM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TABNAM
            {
                get
                {
                    return this._tABNAM;
                }
                set
                {
                    this._tABNAM = value;
                }
            }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("MANDT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MANDT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(16)]
            [System.Xml.Serialization.XmlElementAttribute("DOCNUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DOCNUM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("DOCREL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DOCREL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("STATUS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STATUS { get; set; }

            [System.Xml.Serialization.XmlElementAttribute("DIRECT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCEDI_DC40DIRECT DIRECT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("OUTMOD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string OUTMOD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("EXPRSS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string EXPRSS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("TEST", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TEST { get; set; }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private string _iDOCTYP = "Z1ZVOE_DEAL_1";

            [System.Xml.Serialization.XmlElementAttribute("IDOCTYP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string IDOCTYP
            {
                get
                {
                    return this._iDOCTYP;
                }
                set
                {
                    this._iDOCTYP = value;
                }
            }

            [System.Xml.Serialization.XmlElementAttribute("CIMTYP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CIMTYP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("MESTYP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MESTYP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("MESCOD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MESCOD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("MESFCT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MESFCT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("STD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("STDVRS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STDVRS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("STDMES", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STDMES { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("SNDPOR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDPOR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("SNDPRT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDPRT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("SNDPFC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDPFC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("SNDPRN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDPRN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(21)]
            [System.Xml.Serialization.XmlElementAttribute("SNDSAD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDSAD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(70)]
            [System.Xml.Serialization.XmlElementAttribute("SNDLAD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SNDLAD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("RCVPOR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVPOR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("RCVPRT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVPRT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("RCVPFC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVPFC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("RCVPRN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVPRN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(21)]
            [System.Xml.Serialization.XmlElementAttribute("RCVSAD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVSAD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(70)]
            [System.Xml.Serialization.XmlElementAttribute("RCVLAD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RCVLAD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("CREDAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CREDAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("CRETIM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CRETIM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(14)]
            [System.Xml.Serialization.XmlElementAttribute("REFINT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REFINT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(14)]
            [System.Xml.Serialization.XmlElementAttribute("REFGRP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REFGRP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(14)]
            [System.Xml.Serialization.XmlElementAttribute("REFMES", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REFMES { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(70)]
            [System.Xml.Serialization.XmlElementAttribute("ARCKEY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ARCKEY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SERIAL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SERIAL { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCEDI_DC40SEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCEDI_DC40DIRECT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCEDI_DC40DIRECT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,

            [System.Xml.Serialization.XmlEnumAttribute("2")]
            Item2,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCEDI_DC40SEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCEDI_DC40SEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEAL", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEAL
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("KUNAG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KUNAG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("VKORG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VKORG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VTWEG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VTWEG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("SPART", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SPART { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("VKBUR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VKBUR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VKGRP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VKGRP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
            [System.Xml.Serialization.XmlElementAttribute("WAERS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string WAERS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("VISPN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VISPN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VTTYP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VTTYP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("VTTYP_NAME", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VTTYP_NAME { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("KZNAK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KZNAK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("VKPRTK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VKPRTK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("PRREL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PRREL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("PRRDAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PRRDAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("SALESP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALESP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("BZIRK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BZIRK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("VERSION_ID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VERSION_ID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("LEAD_SOURCE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAD_SOURCE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("ERNAM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ERNAM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("LEASING_DESK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEASING_DESK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("KUNAG_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KUNAG_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(100)]
            [System.Xml.Serialization.XmlElementAttribute("EMAIL_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string EMAIL_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_RLIST", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_RLIST { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("GANUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string GANUM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("KUNVT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KUNVT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("SOURCE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SOURCE { get; set; }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPA> _z1ZVOE_SEPA;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_SEPA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPA> Z1ZVOE_SEPA
            {
                get
                {
                    return this._z1ZVOE_SEPA;
                }
                private set
                {
                    this._z1ZVOE_SEPA = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_SEPASpecified
            {
                get
                {
                    return (this.Z1ZVOE_SEPA.Count != 0);
                }
            }

            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEAL()
            {
                this._z1ZVOE_SEPA = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPA>();
                this._z1ZVOE_COMMISSION_BNL = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNL>();
                this._z1ZVOE_COMMISSION_BF = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BF>();
                this._z1ZVOE_COMMISSION_BD = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BD>();
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNL> _z1ZVOE_COMMISSION_BNL;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_COMMISSION_BNL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNL> Z1ZVOE_COMMISSION_BNL
            {
                get
                {
                    return this._z1ZVOE_COMMISSION_BNL;
                }
                private set
                {
                    this._z1ZVOE_COMMISSION_BNL = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_COMMISSION_BNLSpecified
            {
                get
                {
                    return (this.Z1ZVOE_COMMISSION_BNL.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BF> _z1ZVOE_COMMISSION_BF;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_COMMISSION_BF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BF> Z1ZVOE_COMMISSION_BF
            {
                get
                {
                    return this._z1ZVOE_COMMISSION_BF;
                }
                set
                {
                    this._z1ZVOE_COMMISSION_BF = value;
                }
                //private set
                //{
                //    this._z1ZVOE_COMMISSION_BF = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_COMMISSION_BFSpecified
            {
                get
                {
                    return (this.Z1ZVOE_COMMISSION_BF.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BD> _z1ZVOE_COMMISSION_BD;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_COMMISSION_BD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BD> Z1ZVOE_COMMISSION_BD
            {
                get
                {
                    return this._z1ZVOE_COMMISSION_BD;
                }
                private set
                {
                    this._z1ZVOE_COMMISSION_BD = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_COMMISSION_BDSpecified
            {
                get
                {
                    return (this.Z1ZVOE_COMMISSION_BD.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPA", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPA
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("COMPANY_CODE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string COMPANY_CODE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("SEPA_VAL_FROM_DATE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SEPA_VAL_FROM_DATE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("SEPA_VAL_TO_DATE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SEPA_VAL_TO_DATE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("SIGN_CITY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SIGN_CITY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("SIGN_DATE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SIGN_DATE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("SND_LANGUAGE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SND_LANGUAGE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(70)]
            [System.Xml.Serialization.XmlElementAttribute("SND_ID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SND_ID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("PAYER_ID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PAYER_ID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(34)]
            [System.Xml.Serialization.XmlElementAttribute("PAYER_IBAN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PAYER_IBAN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(11)]
            [System.Xml.Serialization.XmlElementAttribute("PAYER_SWIFT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PAYER_SWIFT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
            [System.Xml.Serialization.XmlElementAttribute("PAYER_ACCOUNT_HOLDER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PAYER_ACCOUNT_HOLDER { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPASEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPASEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_SEPASEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNL", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNL
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("VISPN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VISPN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_VK_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_VK_2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_RLIST", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_RLIST { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_VK1_PC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_VK1_PC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_VK2_PC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_VK2_PC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_SPFF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_SPFF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_LPFF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_LPFF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_LPSF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_LPSF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_SPSF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_SPSF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BNL_BOFF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BNL_BOFF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("MOVEMENT_FLAT_CHARGE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MOVEMENT_FLAT_CHARGE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_SERVICE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_SERVICE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("ZPROVHEXA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZPROVHEXA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_PRICE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_PRICE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_PRICE_SOLUTION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_PRICE_SOLUTION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_SOLUTION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_SOLUTION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("KZSPR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KZSPR { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNLSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNLSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BNLSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BF", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BF
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_SALES_PERSON", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_SALES_PERSON { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_VKL_1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_VKL_1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_VKL_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_VKL_2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_DIR_1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_DIR_1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_AST_1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_AST_1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_AST_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_AST_2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("BF_NEW_CUSTOMER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_NEW_CUSTOMER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("BF_PRADAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_PRADAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(7)]
            [System.Xml.Serialization.XmlElementAttribute("BF_CAVALUE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_CAVALUE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("BF_X_MARCHE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_X_MARCHE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("BF_X_BEU_SUPPORT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BF_X_BEU_SUPPORT { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BFSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BFSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BFSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BD", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BD
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("VISPN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VISPN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("MOVEMENT_FLAT_CHARGE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MOVEMENT_FLAT_CHARGE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_SERVICE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_SERVICE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("ZPROVHEXA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZPROVHEXA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_PRICE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_PRICE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_PRICE_SOLUTION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_PRICE_SOLUTION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_SOLUTION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_SOLUTION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BBE_LANDED_COSTS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BBE_LANDED_COSTS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BBE_INVEST", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BBE_INVEST { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("BBE_MARGIN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BBE_MARGIN { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BDSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BDSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALZ1ZVOE_COMMISSION_BDSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_DEALSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("CONTR_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTR_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("VT_AUART", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_AUART { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("VT_BEGDA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_BEGDA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ABNDA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ABNDA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VTART", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VTART { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VTART_NAME", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VTART_NAME { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VT_SERWI", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_SERWI { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ESCAL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ESCAL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_SWMTN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_SWMTN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_AUGRU", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_AUGRU { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_LAUFK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_LAUFK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VLAUFZ", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VLAUFZ { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VLAUFE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VLAUFE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_FAKSP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_FAKSP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_FAKSK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_FAKSK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("VT_KTEXT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_KTEXT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_KEIN_FPLAN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_KEIN_FPLAN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ANZPOS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ANZPOS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VT_SAP_CONTRACT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_SAP_CONTRACT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VUNDAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VUNDAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ZTERM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ZTERM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ZZ_ZTERM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ZZ_ZTERM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("VT_BEDAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_BEDAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VTNUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VTNUM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VTPOS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VTPOS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("VT_BSTKD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_BSTKD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("CONTR_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTR_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("VT_BSTKD_E", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_BSTKD_E { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("VT_IHREZ_E", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_IHREZ_E { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_REPAIR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_REPAIR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_CALLB", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_CALLB { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(13)]
            [System.Xml.Serialization.XmlElementAttribute("VT_OPS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_OPS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_PERIO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_PERIO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_AUTTE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_AUTTE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_ZZ_NOPRA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_ZZ_NOPRA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_KEIN_FPLAN_CLICK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_KEIN_FPLAN_CLICK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VT_SPLIT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_SPLIT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VT_SAP_CONTRACT_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_SAP_CONTRACT_2 { get; set; }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMS> _z1ZVOE_CONTRACT_ITEMS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_CONTRACT_ITEMS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMS> Z1ZVOE_CONTRACT_ITEMS
            {
                get
                {
                    return this._z1ZVOE_CONTRACT_ITEMS;
                }
                private set
                {
                    this._z1ZVOE_CONTRACT_ITEMS = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_CONTRACT_ITEMSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_CONTRACT_ITEMS.Count != 0);
                }
            }

            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS()
            {
                this._z1ZVOE_CONTRACT_ITEMS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMS>();
                this._z1ZVOE_MATKL = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKL>();
                this._z1ZVOE_KONDM = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDM>();
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKL> _z1ZVOE_MATKL;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_MATKL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKL> Z1ZVOE_MATKL
            {
                get
                {
                    return this._z1ZVOE_MATKL;
                }
                private set
                {
                    this._z1ZVOE_MATKL = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_MATKLSpecified
            {
                get
                {
                    return (this.Z1ZVOE_MATKL.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDM> _z1ZVOE_KONDM;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_KONDM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDM> Z1ZVOE_KONDM
            {
                get
                {
                    return this._z1ZVOE_KONDM;
                }
                private set
                {
                    this._z1ZVOE_KONDM = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_KONDMSpecified
            {
                get
                {
                    return (this.Z1ZVOE_KONDM.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_ITM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_ITM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VTART", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VTART { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("VT_VLAUFZ", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_VLAUFZ { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("VT_LAUFK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VT_LAUFK { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_CONTRACT_ITEMSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKL", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKL
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(9)]
            [System.Xml.Serialization.XmlElementAttribute("MATKL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MATKL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKLSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKLSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_MATKLSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDM", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDM
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("KONDM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KONDM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDMSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDMSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSZ1ZVOE_KONDMSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("DOC_TYPE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DOC_TYPE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("REQ_DATE_H", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REQ_DATE_H { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("PURCH_DATE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PURCH_DATE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("PO_METHOD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_METHOD { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("PO_SUPPLEM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_SUPPLEM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("REF_1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REF_1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("NAME", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(16)]
            [System.Xml.Serialization.XmlElementAttribute("TELEPHONE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TELEPHONE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GROUP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GROUP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_DIST", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_DIST { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("DLV_BLOCK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DLV_BLOCK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("BILL_BLOCK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BILL_BLOCK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("ORD_REASON", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ORD_REASON { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("COMPL_DLV", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string COMPL_DLV { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("PURCH_NO_C", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PURCH_NO_C { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("PURCH_NO_S", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PURCH_NO_S { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("PO_DAT_S", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_DAT_S { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("PO_METH_S", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_METH_S { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("REF_1_S", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REF_1_S { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("SHIP_COND", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SHIP_COND { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("PMNTTRMS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PMNTTRMS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("ORDCOMB_IN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ORDCOMB_IN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("INCOTERMS1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string INCOTERMS1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(28)]
            [System.Xml.Serialization.XmlElementAttribute("INCOTERMS2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string INCOTERMS2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GRP1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GRP1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GRP2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GRP2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GRP3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GRP3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GRP4", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GRP4 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_GRP5", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_GRP5 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_ITM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_ITM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("KZUHG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KZUHG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(18)]
            [System.Xml.Serialization.XmlElementAttribute("MACHINE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MACHINE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("DWERK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DWERK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("USED_MACHINE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string USED_MACHINE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(18)]
            [System.Xml.Serialization.XmlElementAttribute("EQUIPMENT_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string EQUIPMENT_NO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("ORDER_FLAG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ORDER_FLAG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ORDER_INFO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ORDER_INFO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
            [System.Xml.Serialization.XmlElementAttribute("INVENTORY_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string INVENTORY_NO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(25)]
            [System.Xml.Serialization.XmlElementAttribute("TECH_ID_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TECH_ID_NO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
            [System.Xml.Serialization.XmlElementAttribute("LINKING_PIN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LINKING_PIN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("PROJECT_VALID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PROJECT_VALID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("REGEL_DP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REGEL_DP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("VALID_DP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VALID_DP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("BSTKD_E", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BSTKD_E { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("IHREZ_E", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string IHREZ_E { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(32)]
            [System.Xml.Serialization.XmlElementAttribute("ZZSWORDER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZZSWORDER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("ZCONRECSER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZCONRECSER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(241)]
            [System.Xml.Serialization.XmlElementAttribute("E_MAIL_AG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string E_MAIL_AG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("ZZ_EBIZORDER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZZ_EBIZORDER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("ZDUM_INFO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZDUM_INFO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("CX2_ORDER_REF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CX2_ORDER_REF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("CX2_ORDER_GUID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CX2_ORDER_GUID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("CX2_SHOPPER_REF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CX2_SHOPPER_REF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(16)]
            [System.Xml.Serialization.XmlElementAttribute("CX2_TOKEN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CX2_TOKEN { get; set; }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEM> _z1ZVOE_COMMISSION_BD_ITEM;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_COMMISSION_BD_ITEM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEM> Z1ZVOE_COMMISSION_BD_ITEM
            {
                get
                {
                    return this._z1ZVOE_COMMISSION_BD_ITEM;
                }
                private set
                {
                    this._z1ZVOE_COMMISSION_BD_ITEM = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_COMMISSION_BD_ITEMSpecified
            {
                get
                {
                    return (this.Z1ZVOE_COMMISSION_BD_ITEM.Count != 0);
                }
            }

            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS()
            {
                this._z1ZVOE_COMMISSION_BD_ITEM = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEM>();
                this._z1ZVOE_ORDER_CONTACT = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>();
                this._z1ZVOE_ORDER_CONTACT_ADD = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADD>();
                this._z1ZVOE_ADD_AGREE = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREE>();
                this._z1ZVOE_ORDER_ITEMS = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                this._z1ZVOE_CLICK_PRICES = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();
                this._z1ZVOE_FINANCE = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT> _z1ZVOE_ORDER_CONTACT;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ORDER_CONTACT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT> Z1ZVOE_ORDER_CONTACT
            {
                get
                {
                    return this._z1ZVOE_ORDER_CONTACT;
                }
                set
                {
                    this._z1ZVOE_ORDER_CONTACT = value;
                }
                //private set
                //{
                //    this._z1ZVOE_ORDER_CONTACT = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ORDER_CONTACTSpecified
            {
                get
                {
                    return (this.Z1ZVOE_ORDER_CONTACT.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADD> _z1ZVOE_ORDER_CONTACT_ADD;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ORDER_CONTACT_ADD", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADD> Z1ZVOE_ORDER_CONTACT_ADD
            {
                get
                {
                    return this._z1ZVOE_ORDER_CONTACT_ADD;
                }
                private set
                {
                    this._z1ZVOE_ORDER_CONTACT_ADD = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ORDER_CONTACT_ADDSpecified
            {
                get
                {
                    return (this.Z1ZVOE_ORDER_CONTACT_ADD.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREE> _z1ZVOE_ADD_AGREE;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ADD_AGREE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREE> Z1ZVOE_ADD_AGREE
            {
                get
                {
                    return this._z1ZVOE_ADD_AGREE;
                }
                private set
                {
                    this._z1ZVOE_ADD_AGREE = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ADD_AGREESpecified
            {
                get
                {
                    return (this.Z1ZVOE_ADD_AGREE.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS> _z1ZVOE_ORDER_ITEMS;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_ORDER_ITEMS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS> Z1ZVOE_ORDER_ITEMS
            {
                get
                {
                    return this._z1ZVOE_ORDER_ITEMS;
                }
                set
                {
                    this._z1ZVOE_ORDER_ITEMS = value;
                }
                //private set
                //{
                //    this._z1ZVOE_ORDER_ITEMS = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_ORDER_ITEMSSpecified
            {
                get
                {
                    return (this.Z1ZVOE_ORDER_ITEMS.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES> _z1ZVOE_CLICK_PRICES;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_CLICK_PRICES", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES> Z1ZVOE_CLICK_PRICES
            {
                get
                {
                    return this._z1ZVOE_CLICK_PRICES;
                }
                set
                {
                    this._z1ZVOE_CLICK_PRICES = value;
                }
                //private set
                //{
                //    this._z1ZVOE_CLICK_PRICES = value;
                //}
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_CLICK_PRICESSpecified
            {
                get
                {
                    return (this.Z1ZVOE_CLICK_PRICES.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            private System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE> _z1ZVOE_FINANCE;

            [System.Xml.Serialization.XmlElementAttribute("Z1ZVOE_FINANCE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE> Z1ZVOE_FINANCE
            {
                get
                {
                    return this._z1ZVOE_FINANCE;
                }
                set
                {
                    this._z1ZVOE_FINANCE = value;
                }
            }

            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool Z1ZVOE_FINANCESpecified
            {
                get
                {
                    return (this.Z1ZVOE_FINANCE.Count != 0);
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEM", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEM
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("A3A4", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string A3A4 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("KZNMA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KZNMA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("LIPRG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LIPRG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("LIPRHAWAP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LIPRHAWAP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_PRICE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_PRICE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("MOVEMENT_FLAT_CHARGE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MOVEMENT_FLAT_CHARGE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_HARDWARE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_HARDWARE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("REPLACEMENT_RATE_SERVICE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REPLACEMENT_RATE_SERVICE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEMSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEMSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_COMMISSION_BD_ITEMSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_NAME", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_NAME { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_PHON", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_PHON { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_OPEN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_OPEN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_NAME", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_NAME { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_PHON", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_PHON { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_OPEN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_OPEN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_INFO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_INFO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_INFO2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_INFO2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_INFO3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_INFO3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO4", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO4 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO5", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO5 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_INFO4", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_INFO4 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APRT_INFO5", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APRT_INFO5 { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACTSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACTSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACTSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADD", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADD
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO6", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO6 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO7", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO7 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO8", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO8 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO9", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO9 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO10", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO10 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO11", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO11 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO12", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO12 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(65)]
            [System.Xml.Serialization.XmlElementAttribute("APLF_INFO13", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string APLF_INFO13 { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADDSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADDSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT_ADDSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREE", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREE
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("FLAG_POS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FLAG_POS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("FLAG1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FLAG1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("FLAG2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FLAG2 { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREESEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREESEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ADD_AGREESEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("HG_LV_ITEM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string HG_LV_ITEM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(18)]
            [System.Xml.Serialization.XmlElementAttribute("MATERIAL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MATERIAL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(22)]
            [System.Xml.Serialization.XmlElementAttribute("CUST_MAT22", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUST_MAT22 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("BATCH", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BATCH { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("PLANT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PLANT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("STORE_LOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STORE_LOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("SHORT_TEXT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SHORT_TEXT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("SALES_UNIT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SALES_UNIT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
            [System.Xml.Serialization.XmlElementAttribute("REQ_QTY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REQ_QTY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("MODEL_YN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MODEL_YN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM_NUMBER_ORIG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM_NUMBER_ORIG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(18)]
            [System.Xml.Serialization.XmlElementAttribute("MATNR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MATNR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("KLFN1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KLFN1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("DATAB", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATAB { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("DATBI", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATBI { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("KSTBM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KSTBM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(13)]
            [System.Xml.Serialization.XmlElementAttribute("KBETR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KBETR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
            [System.Xml.Serialization.XmlElementAttribute("KONWA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KONWA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("POINT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string POINT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("GRUPPE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string GRUPPE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("ZZPOOL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZZPOOL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(17)]
            [System.Xml.Serialization.XmlElementAttribute("KSTBM_MIN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KSTBM_MIN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("ZLEVEL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZLEVEL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(9)]
            [System.Xml.Serialization.XmlElementAttribute("ZPERC_CLICK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZPERC_CLICK { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICESSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICESSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICESSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("FINANCE_TYPE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FINANCE_TYPE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(13)]
            [System.Xml.Serialization.XmlElementAttribute("KBETR1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KBETR1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(13)]
            [System.Xml.Serialization.XmlElementAttribute("KBETR2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KBETR2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("KZ_NULLPR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KZ_NULLPR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_KUNNR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_KUNNR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LVTNR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LVTNR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LEABG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LEABG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LEPER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LEPER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LRYTH", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LRYTH { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(9)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LFAKT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LFAKT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(9)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_LKAUP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_LKAUP { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("LEAS_ZTERM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LEAS_ZTERM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("ANLKL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ANLKL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("RWERK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string RWERK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("LGORT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LGORT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("PAYER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PAYER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("BILL_TO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BILL_TO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_RE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_RE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_RG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_RG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_ZV", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_ZV { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CONTRACT_ZW", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTRACT_ZW { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCESEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCESEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCESEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("SD_ITM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_ITM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("TXTID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TXTID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("SPRAS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SPRAS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("TDPOS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TDPOS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("TDFORMAT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TDFORMAT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(132)]
            [System.Xml.Serialization.XmlElementAttribute("TDLINE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TDLINE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_TEXTSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("PARTN_ROLE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PARTN_ROLE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CUSTOMER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CUSTOMER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("VENDOR_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VENDOR_NO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("PERSON_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PERSON_NO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("CONTACT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CONTACT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("COUNTRY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string COUNTRY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SO_BUILDING", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SO_BUILDING { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("SO_FLOOR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SO_FLOOR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("SO_ROOMNR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SO_ROOMNR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("SO_DEPARTMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SO_DEPARTMENT { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
            [System.Xml.Serialization.XmlElementAttribute("SO_REMARK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SO_REMARK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("CP_NAMEV", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_NAMEV { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(35)]
            [System.Xml.Serialization.XmlElementAttribute("CP_NAME1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_NAME1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("CP_ANRED", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_ANRED { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("CP_PHONE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_PHONE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("CP_MOBILE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_MOBILE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(241)]
            [System.Xml.Serialization.XmlElementAttribute("CP_MAIL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CP_MAIL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER_CRM { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("NATION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NATION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("DATE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("DATE_FROM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATE_FROM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("DATE_TO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATE_TO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("TITLE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TITLE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("NAME1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("NAME2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("NAME3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("NAME4", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME4 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("NAME_CO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string NAME_CO { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("CITY1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CITY1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("CITY2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CITY2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("CITY_CODE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CITY_CODE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(8)]
            [System.Xml.Serialization.XmlElementAttribute("CITYP_CODE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CITYP_CODE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("CHCKSTATUS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CHCKSTATUS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("POST_CODE1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string POST_CODE1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("POST_CODE2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string POST_CODE2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("POST_CODE3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string POST_CODE3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("PO_BOX", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_BOX { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("PO_BOX_NUM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_BOX_NUM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("PO_BOX_LOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_BOX_LOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("CITY_CODE2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string CITY_CODE2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("PO_BOX_REG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_BOX_REG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("PO_BOX_CTY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string PO_BOX_CTY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(15)]
            [System.Xml.Serialization.XmlElementAttribute("POSTALAREA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string POSTALAREA { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("TRANSPZONE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TRANSPZONE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(60)]
            [System.Xml.Serialization.XmlElementAttribute("STREET", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STREET { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(12)]
            [System.Xml.Serialization.XmlElementAttribute("STREETCODE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STREETCODE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(2)]
            [System.Xml.Serialization.XmlElementAttribute("STREETABBR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STREETABBR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("HOUSE_NUM1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string HOUSE_NUM1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("HOUSE_NUM2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string HOUSE_NUM2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("HOUSE_NUM3", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string HOUSE_NUM3 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("STR_SUPPL1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string STR_SUPPL1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("LOCATION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LOCATION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("BUILDING", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BUILDING { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("FLOOR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FLOOR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("ROOMNUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ROOMNUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("COUNTRY", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string COUNTRY { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("LANGU", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string LANGU { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("REGION", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REGION { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SORT1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SORT1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("SORT2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SORT2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRORIGIN", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRORIGIN { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("EXTENSION1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string EXTENSION1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("EXTENSION2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string EXTENSION2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("TIME_ZONE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TIME_ZONE { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(50)]
            [System.Xml.Serialization.XmlElementAttribute("REMARK", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string REMARK { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(3)]
            [System.Xml.Serialization.XmlElementAttribute("DEFLT_COMM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DEFLT_COMM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("TEL_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TEL_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("TEL_EXTENS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TEL_EXTENS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("FAX_NUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FAX_NUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("FAX_EXTENS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string FAX_EXTENS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("BUILD_LONG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string BUILD_LONG { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSESSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSESSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSESSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(10)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(241)]
            [System.Xml.Serialization.XmlElementAttribute("E_MAIL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string E_MAIL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(16)]
            [System.Xml.Serialization.XmlElementAttribute("TAX_NO_1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TAX_NO_1 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(11)]
            [System.Xml.Serialization.XmlElementAttribute("TAX_NO_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string TAX_NO_2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER_2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER_2 { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(241)]
            [System.Xml.Serialization.XmlElementAttribute("SMTP_ADDR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SMTP_ADDR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("ADDRNUMBER_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ADDRNUMBER_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("ZZCX2_REF", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZZCX2_REF { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(40)]
            [System.Xml.Serialization.XmlElementAttribute("ZZCX2_GUID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ZZCX2_GUID { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("VAT_REG_NO", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string VAT_REG_NO { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADDSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADDSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADDSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(20)]
            [System.Xml.Serialization.XmlElementAttribute("DOC", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DOC { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(6)]
            [System.Xml.Serialization.XmlElementAttribute("ITM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string ITM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(1)]
            [System.Xml.Serialization.XmlElementAttribute("COND_FLAG", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string COND_FLAG { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)]
            [System.Xml.Serialization.XmlElementAttribute("KSCHL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KSCHL { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(13)]
            [System.Xml.Serialization.XmlElementAttribute("KBETR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string KBETR { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(5)]
            [System.Xml.Serialization.XmlElementAttribute("WAERS", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string WAERS { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("SD_DOC_CRM", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string SD_DOC_CRM { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(18)]
            [System.Xml.Serialization.XmlElementAttribute("MATNR", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string MATNR { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONSSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONSSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONSSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENT", Namespace = "", AnonymousType = true)]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        public partial class Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENT
        {

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(30)]
            [System.Xml.Serialization.XmlElementAttribute("IDENTIFIER", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string IDENTIFIER { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLengthAttribute(970)]
            [System.Xml.Serialization.XmlElementAttribute("DATA", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string DATA { get; set; }

            [System.Xml.Serialization.XmlAttributeAttribute("SEGMENT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENTSEGMENT SEGMENT { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENTSEGMENT", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCE1IDOCENHANCEMENTSEGMENT
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }

        [System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "2.0.210.0")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute("Z1ZVOE_DEAL_1IDOCBEGIN", Namespace = "")]
        public enum Z1ZVOE_DEAL_1IDOCBEGIN
        {

            [System.Xml.Serialization.XmlEnumAttribute("1")]
            Item1,
        }
    }




}

