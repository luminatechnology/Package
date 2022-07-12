using System;
using PX.Data;
using PX.Objects.GL;
using eGUICustomizations.Descriptor;
using PX.Data.ReferentialIntegrity.Attributes;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("GUI Prepay Adjustment")]
    public class TWNGUIPrepayAdjust : IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<TWNGUIPrepayAdjust>.By<prepayGUINbr, appliedGUINbr, sequenceNo, reason>
        {
            public static TWNGUIPrepayAdjust Find(PXGraph graph, string prepayGUINbr, string appliedGUINbr, int? sequenceNo, string reason) => FindBy(graph, prepayGUINbr, appliedGUINbr, sequenceNo, reason);
        }
        #endregion

        #region PrepayGUINbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Prepay GUI Nbr.")]
        public virtual string PrepayGUINbr { get; set; }
        public abstract class prepayGUINbr : PX.Data.BQL.BqlString.Field<prepayGUINbr> { }
        #endregion

        #region AppliedGUINbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Applied GUI Nbr.")]
        public virtual string AppliedGUINbr { get; set; }
        public abstract class appliedGUINbr : PX.Data.BQL.BqlString.Field<appliedGUINbr> { }
        #endregion

        #region SequenceNo
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Sequence No")]
        public virtual int? SequenceNo { get; set; }
        public abstract class sequenceNo : PX.Data.BQL.BqlInt.Field<sequenceNo> { }
        #endregion

        #region Reason
        [PXDBString(1, IsKey = true, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Reason")]
        [TWNStringList.TWNGUIStatus.List]
        public virtual string Reason { get; set; }
        public abstract class reason : PX.Data.BQL.BqlString.Field<reason> { }
        #endregion

        #region BranchID
        [Branch()]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region NetAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual decimal? NetAmt { get; set; }
        public abstract class netAmt : PX.Data.BQL.BqlDecimal.Field<netAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Tax Amount")]
        public virtual decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region NetAmtUnapplied
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Net Amount Unapplied")]
        public virtual Decimal? NetAmtUnapplied { get; set; }
        public abstract class netAmtUnapplied : PX.Data.BQL.BqlDecimal.Field<netAmtUnapplied> { }
        #endregion

        #region TaxAmtUnapplied
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Tax Amount Unapplied")]
        public virtual decimal? TaxAmtUnapplied { get; set; }
        public abstract class taxAmtUnapplied : PX.Data.BQL.BqlDecimal.Field<taxAmtUnapplied> { }
        #endregion

        #region Remark
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}