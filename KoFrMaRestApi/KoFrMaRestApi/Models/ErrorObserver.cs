using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class ErrorObserver
    {
        private static ErrorObserver instance = null;
        private List<Exception> _exceptions = new List<Exception>();
        public List<Exception> Exceptions { get {
                return _exceptions;
            }}
        public static ErrorObserver GetInstance()
        {
            if (ErrorObserver.instance == null)
                ErrorObserver.instance = new ErrorObserver();
            return ErrorObserver.instance;
        }
        private ErrorObserver()
        {
        }
        public void RegisterError(Exception exception)
        {
            _exceptions.Add(exception);
        }
    }
}