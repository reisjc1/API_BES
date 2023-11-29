using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ErrorCodes
    {
        public enum ErrorCode_SUCESS
        {
            Err = 0,
            //Message = "ProposalID";
        }

        public enum ErrorCode_FAILED
        {
            Err = 1,
            //Message = "Failed";
        }
    }
}