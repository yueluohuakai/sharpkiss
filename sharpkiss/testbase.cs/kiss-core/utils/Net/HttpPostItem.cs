
using System.Web;

namespace Kiss.Utils.Net
{
    public class HttpPostItem
    {
        private string _parameter = null;
        private string _value = null;
        private bool _isEncoded = false;

        public HttpPostItem( string paramter, string postValue, bool isEncoded )
        {
            _parameter = paramter;
            _value = postValue;
            _isEncoded = isEncoded;
        }

        public HttpPostItem( string paramter, string postValue )
            : this( paramter, postValue, false )
        {

        }

        public override string ToString()
        {
            return string.Format( "{0}={1}", _parameter, _isEncoded ? _value : HttpUtility.UrlDecode( _value ) );
        }
    }
}
