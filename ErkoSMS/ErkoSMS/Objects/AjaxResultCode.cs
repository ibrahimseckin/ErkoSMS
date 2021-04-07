


namespace ErkoSMS.Objects
{

    /// <summary>
    /// Defines the possible return codes from an AJAX call
    /// Mirrors RCMS.ResultCode enum in the front-end code.
    /// Any changes made to this enum must be reflected to the front-end.
    /// </summary>
    public enum AjaxResultCode
    {
        None = 0,
        Success = 1,
        UserFailure = 2,
        NotLoggedIn = 3
    }
}
