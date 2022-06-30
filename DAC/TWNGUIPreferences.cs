using System;
using PX.Data;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.IN;
using eGUICustomizations.Graph;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXPrimaryGraph(typeof(TWNGUIPrefMaint))]
    [PXCacheName("GUI Preferences")]
    public class TWNGUIPreferences : PX.Data.IBqlTable
    {
        #region GUI3CopiesNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "3 Copies e-GUI Numbering Sequence", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string GUI3CopiesNumbering { get; set; }
        public abstract class gUI3CopiesNumbering : PX.Data.BQL.BqlString.Field<gUI3CopiesNumbering> { }
        #endregion
    
        #region GUI2CopiesNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "2 Copies GUI Numbering Sequence", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string GUI2CopiesNumbering { get; set; }
        public abstract class gUI2CopiesNumbering : PX.Data.BQL.BqlString.Field<gUI2CopiesNumbering> { }
        #endregion
    
        #region MediaFileNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Media File Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string MediaFileNumbering { get; set; }
        public abstract class mediaFileNumbering : PX.Data.BQL.BqlString.Field<mediaFileNumbering> { }
        #endregion

        #region RequestNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Request Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string RequestNumbering { get; set; }
        public abstract class requestNumbering : PX.Data.BQL.BqlString.Field<requestNumbering> { }
        #endregion

        #region WHTFileNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Withholding File Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string WHTFileNumbering { get; set; }
        public abstract class wHTFileNumbering : PX.Data.BQL.BqlString.Field<wHTFileNumbering> { }
        #endregion

        #region GUI3CopiesManNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "3 Copies GUI Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        public virtual string GUI3CopiesManNumbering { get; set; }
        public abstract class gUI3CopiesManNumbering : PX.Data.BQL.BqlString.Field<gUI3CopiesManNumbering> { }
        #endregion

        #region TaxRegistrationID
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Registration")]
        public virtual string TaxRegistrationID { get; set; }
        public abstract class taxRegistrationID : PX.Data.BQL.BqlString.Field<taxRegistrationID> { }
        #endregion
    
        #region OurTaxNbr
        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Our Tax Nbr")]
        public virtual string OurTaxNbr { get; set; }
        public abstract class ourTaxNbr : PX.Data.BQL.BqlString.Field<ourTaxNbr> { }
        #endregion
    
        #region ZeroTaxTaxCntry
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Zero Tax County")]
        public virtual string ZeroTaxTaxCntry { get; set; }
        public abstract class zeroTaxTaxCntry : PX.Data.BQL.BqlString.Field<zeroTaxTaxCntry> { }
        #endregion

        #region UserName
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "User Name", Visibility = PXUIVisibility.Visible)]
        [PXDefault]
        public virtual string UserName { get; set; }
        public abstract class userName : PX.Data.BQL.BqlString.Field<userName> { }
        #endregion

        #region Password
        [PXRSACryptString(IsUnicode = true)]
        [PXUIField(DisplayName = "Password", Visibility = PXUIVisibility.Visible)]
        [PXDefault]
        public virtual string Password { get; set; }
        public abstract class password : PX.Data.BQL.BqlString.Field<password> { }
        #endregion

        #region Url
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "URL", Visibility = PXUIVisibility.Visible)]
        [PXDefault]
        public virtual string Url { get; set; }
        public abstract class url : PX.Data.BQL.BqlString.Field<url> { }
        #endregion

        #region CompanyName      
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Company Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        [PXPersonalDataField]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
        #endregion

        #region AddressLine       
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Company Address")]
        [PXMassMergableField]
        [PXPersonalDataField]
        public virtual string AddressLine { get; set; }
        public abstract class addressLine : PX.Data.BQL.BqlString.Field<addressLine> { }
        #endregion

        #region AESKey
        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "AES Key")]
        public virtual string AESKey { get; set; }
        public abstract class aESKey : PX.Data.BQL.BqlString.Field<aESKey> { }
        #endregion

        #region PlasticBag
        [NonStockItem(Visibility = PXUIVisibility.SelectorVisible, IsDirty = true)]
        [PXUIField(DisplayName = "Plastic Bag")]
        public virtual int? PlasticBag { get; set; }
        public abstract class plasticBag : PX.Data.BQL.BqlInt.Field<plasticBag> { }
        #endregion

        #region EnableWHT
        [PXDBBool()]
        [PXUIField(DisplayName = "Enable Withholding Tax")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EnableWHT { get; set; }
        public abstract class enableWHT : PX.Data.BQL.BqlBool.Field<enableWHT> { }
        #endregion

        #region WHTTaxAuthority
        [PXDBString(3, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Withholding Tax Authority")]
        public virtual string WHTTaxAuthority { get; set; }
        public abstract class wHTTaxAuthority : PX.Data.BQL.BqlString.Field<wHTTaxAuthority> { }
        #endregion

        #region SecGenerationNHIPct
        [PXDBDecimal(3)]
        [PXUIField(DisplayName = "Second Generation NHI %")]
        public virtual decimal? SecGenerationNHIPct { get; set; }
        public abstract class secGenerationNHIPct : PX.Data.BQL.BqlDecimal.Field<secGenerationNHIPct> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime,
                   Enabled = false,
                   IsReadOnly = true)]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID       
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime,
                   Enabled = false,
                   IsReadOnly = true)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region tstamp        
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        #endregion
    }
}