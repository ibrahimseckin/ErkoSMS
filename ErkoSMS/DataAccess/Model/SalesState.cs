namespace ErkoSMS.DataAccess.Model
{
    public enum SalesState
    {
        //Nonspecified is for old data
        Nonspecified = -1,
        InternalSalesConducted = 0,
        InvoiceDoneAndPacked = 1,
        WaitForPacking = 2,
        WaitForTransfer = 3,
        PaymentDoneAndWaitForTransfer = 4,
        PackingIsReady = 5,
        PackingIsReadyAndWaitForPayment = 6,
        Rejected = 7,
        WaitForAnswer = 8
    }
}
