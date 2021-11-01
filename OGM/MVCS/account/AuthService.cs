
using System.IO;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using XTC.oelMVCS;
using System.Security.Cryptography;
using System.Text;

namespace ogm.account
{
    public class AuthService : Service
    {
        public const string NAME = "AuthService";
        private AuthModel model = null;
        public string domain { get; set; }

        protected override void preSetup()
        {
            model = findModel(AuthModel.NAME) as AuthModel;
        }

        protected override void setup()
        {
            getLogger().Trace("setup AuthService");
        }

        public void PostSignup(Proto.SignupRequest _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["username"] = _request._username;
            paramMap["password"] = Any.FromString(wrapPassword(_request._password.AsString()));

            post(string.Format("{0}/xtc/ogm/account/Auth/Signup", getConfig()["domain"].AsString()), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.SignupResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.Broadcast("/ogm/account/Auth/Signup", reply);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
            }, null);
        }


        public void PostSignin(Proto.SigninRequest _request, string _location, string _domain)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["strategy"] = _request._strategy;
            paramMap["username"] = _request._username;
            paramMap["password"] = Any.FromString(wrapPassword(_request._password.AsString()));
            string location = _location;
            string host = _domain;

            post(string.Format("{0}/xtc/ogm/account/Auth/Signin", _domain), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.SigninResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.UpdateSigninReply(reply, rsp._accessToken.AsString(), rsp._uuid.AsString(), location, host);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
                Model.Status reply = Model.Status.New<Model.Status>(_err.getCode(), _err.getMessage());
                model.UpdateSigninReply(reply, "", "", location, host);
            }, null);
        }

        public void PostSignout(Proto.SignoutRequest _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["strategy"] = _request._strategy;
            paramMap["accessToken"] = _request._accessToken;

            post(string.Format("{0}/xtc/ogm/account/Auth/Signout", domain), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.SignoutResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.Broadcast("/ogm/account/Auth/Signout", reply);
            }, (_err) =>
            {
                getLogger().Error(_err.getMessage());
            }, null);
        }


        public void PostResetPasswd(Proto.ResetPasswdRequest _request)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["strategy"] = _request._strategy;
            paramMap["accessToken"] = _request._accessToken;
            paramMap["password"] = _request._password;

            post(string.Format("{0}/xtc/ogm/account/Auth/ResetPasswd", domain), paramMap, (_reply) =>
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var rsp = JsonSerializer.Deserialize<Proto.ResetPasswdResponse>(_reply, options);
                Model.Status reply = Model.Status.New<Model.Status>(rsp._status._code.AsInt32(), rsp._status._message.AsString());
                model.Broadcast("/ogm/account/Auth/ResetPasswd", reply);
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

        private string wrapPassword(string _password)
        {
            string password = reverseString(_password);
            password = toMd5(password).ToUpper();
            password = reverseString(password);
            password = toMd5(password).ToUpper();
            return password;
        }

        private string toMd5(string _value)
        {
            MD5 md5 = MD5.Create();
            byte[] byteOld = Encoding.UTF8.GetBytes(_value);
            byte[] byteNew = md5.ComputeHash(byteOld);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private string reverseString(string _value)
        {
            char[] cs = _value.ToCharArray();
            System.Array.Reverse(cs);
            return new string(cs);
        }

    }
}
