

namespace ErkoSMS.Objects
{
    public class AjaxResult
    {
        public object Data { get; }
        public AjaxResultCode Code { get; }
        public string Message { get; } = "";

        public AjaxResult()
        {
            Code = AjaxResultCode.Success;
        }

        public AjaxResult(AjaxResultCode code) : this(code, "") { }

        public AjaxResult(AjaxResultCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public AjaxResult(AjaxResultCode code, object data)
        {
            Code = code;
            Data = data;
        }

        public AjaxResult(AjaxResultCode code, object data, string message) : this(code, data)
        {
            Message = message;
        }
        public AjaxResult(object data)
        {
            Code = AjaxResultCode.Success;
            Data = data;
        }
    }
}
