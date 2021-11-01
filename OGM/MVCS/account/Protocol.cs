
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace ogm.account.Proto
{

        public class SignupRequest
        {
            public SignupRequest()
            {
                _username = Any.FromString("");
                _password = Any.FromString("");

            }
            [JsonPropertyName("username")]
            public Any _username {get;set;}
            [JsonPropertyName("password")]
            public Any _password {get;set;}

        }
    
        public class SignupResponse
        {
            public SignupResponse()
            {
                _status = new Status();
                _uuid = Any.FromString("");

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}
            [JsonPropertyName("uuid")]
            public Any _uuid {get;set;}

        }
    
        public class SigninRequest
        {
            public SigninRequest()
            {
                _strategy = Any.FromInt32(0);
                _username = Any.FromString("");
                _password = Any.FromString("");

            }
            [JsonPropertyName("strategy")]
            public Any _strategy {get;set;}
            [JsonPropertyName("username")]
            public Any _username {get;set;}
            [JsonPropertyName("password")]
            public Any _password {get;set;}

        }
    
        public class SigninResponse
        {
            public SigninResponse()
            {
                _status = new Status();
                _uuid = Any.FromString("");
                _accessToken = Any.FromString("");

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}
            [JsonPropertyName("uuid")]
            public Any _uuid {get;set;}
            [JsonPropertyName("accessToken")]
            public Any _accessToken {get;set;}

        }
    
        public class SignoutRequest
        {
            public SignoutRequest()
            {
                _strategy = Any.FromInt32(0);
                _accessToken = Any.FromString("");

            }
            [JsonPropertyName("strategy")]
            public Any _strategy {get;set;}
            [JsonPropertyName("accessToken")]
            public Any _accessToken {get;set;}

        }
    
        public class SignoutResponse
        {
            public SignoutResponse()
            {
                _status = new Status();

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}

        }
    
        public class ResetPasswdRequest
        {
            public ResetPasswdRequest()
            {
                _strategy = Any.FromInt32(0);
                _accessToken = Any.FromString("");
                _password = Any.FromString("");

            }
            [JsonPropertyName("strategy")]
            public Any _strategy {get;set;}
            [JsonPropertyName("accessToken")]
            public Any _accessToken {get;set;}
            [JsonPropertyName("password")]
            public Any _password {get;set;}

        }
    
        public class ResetPasswdResponse
        {
            public ResetPasswdResponse()
            {
                _status = new Status();

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}

        }
    
        public class QueryProfileRequest
        {
            public QueryProfileRequest()
            {
                _strategy = Any.FromInt32(0);
                _accessToken = Any.FromString("");

            }
            [JsonPropertyName("strategy")]
            public Any _strategy {get;set;}
            [JsonPropertyName("accessToken")]
            public Any _accessToken {get;set;}

        }
    
        public class QueryProfileResponse
        {
            public QueryProfileResponse()
            {
                _status = new Status();
                _profile = Any.FromString("");

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}
            [JsonPropertyName("profile")]
            public Any _profile {get;set;}

        }
    
        public class UpdateProfileRequest
        {
            public UpdateProfileRequest()
            {
                _strategy = Any.FromInt32(0);
                _accessToken = Any.FromString("");
                _profile = Any.FromString("");

            }
            [JsonPropertyName("strategy")]
            public Any _strategy {get;set;}
            [JsonPropertyName("accessToken")]
            public Any _accessToken {get;set;}
            [JsonPropertyName("profile")]
            public Any _profile {get;set;}

        }
    
        public class UpdateProfileResponse
        {
            public UpdateProfileResponse()
            {
                _status = new Status();

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}

        }
    
        public class QueryListRequest
        {
            public QueryListRequest()
            {
                _offset = Any.FromInt64(0);
                _count = Any.FromInt64(0);

            }
            [JsonPropertyName("offset")]
            public Any _offset {get;set;}
            [JsonPropertyName("count")]
            public Any _count {get;set;}

        }
    
        public class QueryListResponse
        {
            public QueryListResponse()
            {
                _status = new Status();
                _total = Any.FromInt64(0);
                _account = new AccountEntity[0];

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}
            [JsonPropertyName("total")]
            public Any _total {get;set;}
            [JsonPropertyName("account")]
            public AccountEntity[] _account {get;set;}

        }
    
        public class QuerySingleRequest
        {
            public QuerySingleRequest()
            {
                _field = Any.FromInt32(0);
                _value = Any.FromString("");

            }
            [JsonPropertyName("field")]
            public Any _field {get;set;}
            [JsonPropertyName("value")]
            public Any _value {get;set;}

        }
    
        public class QuerySingleResponse
        {
            public QuerySingleResponse()
            {
                _status = new Status();
                _account = new AccountEntity();

            }
            [JsonPropertyName("status")]
            public Status _status {get;set;}
            [JsonPropertyName("account")]
            public AccountEntity _account {get;set;}

        }
    
        public class Status
        {
            public Status()
            {
                _code = Any.FromInt32(0);
                _message = Any.FromString("");

            }
            [JsonPropertyName("code")]
            public Any _code {get;set;}
            [JsonPropertyName("message")]
            public Any _message {get;set;}

        }
    
        public class AccountEntity
        {
            public AccountEntity()
            {
                _username = Any.FromString("");
                _uuid = Any.FromString("");
                _profile = Any.FromString("");
                _createdAt = Any.FromInt64(0);
                _updatedAt = Any.FromInt64(0);

            }
            [JsonPropertyName("username")]
            public Any _username {get;set;}
            [JsonPropertyName("uuid")]
            public Any _uuid {get;set;}
            [JsonPropertyName("profile")]
            public Any _profile {get;set;}
            [JsonPropertyName("createdAt")]
            public Any _createdAt {get;set;}
            [JsonPropertyName("updatedAt")]
            public Any _updatedAt {get;set;}

        }
    
}
