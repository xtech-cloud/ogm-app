
using System.IO;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace ogm.account
{
    public class QueryService: Service
    {
        public const string NAME = "QueryService";
        private QueryModel model = null;

        protected override void preSetup()
        {
            model = findModel(QueryModel.NAME) as QueryModel;
        }

        protected override void setup()
        {
            getLogger().Trace("setup QueryService");
        }

        public void PostList(Proto.QueryListRequest _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["offset"] = _request._offset;
            paramMap["count"] = _request._count;

            post(string.Format("{0}/ogm/account/Query/List", getConfig().getField("domain").AsString()), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.QueryListResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.Broadcast("/ogm/account/Query/List", reply);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
            }, null);
        }
        

        public void PostSingle(Proto.QuerySingleRequest _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["field"] = _request._field;
            paramMap["value"] = _request._value;

            post(string.Format("{0}/ogm/account/Query/Single", getConfig().getField("domain").AsString()), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.QuerySingleResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.Broadcast("/ogm/account/Query/Single", reply);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
            }, null);
        }
        


        protected override void asyncRequest(string _url, string _method, Dictionary<string, Any> _params, OnReplyCallback _onReply, OnErrorCallback _onError, Options _options)
        {
            string reply = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_url);
                req.Method = _method;
                req.ContentType =
                "application/json;charset=utf-8";
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                byte[] data = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(_params, options);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                if (rsp == null)
                {
                    _onError(Error.NewNullErr("HttpWebResponse is null"));
                    return;
                }
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    rsp.Close();
                    _onError(new Error(rsp.StatusCode.GetHashCode(), "HttpStatusCode != 200"));
                    return;
                }
                StreamReader sr;
                using (sr = new StreamReader(rsp.GetResponseStream()))
                {
                    reply = sr.ReadToEnd();
                }
                sr.Close();
            }
            catch (System.Exception ex)
            {
                _onError(Error.NewException(ex));
                return;
            }
            _onReply(reply);
        }
    }
}
