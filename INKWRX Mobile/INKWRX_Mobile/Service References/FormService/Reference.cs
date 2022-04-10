﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace INKWRX_Mobile.FormService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://destinywireless.com/", ConfigurationName="FormService.IDestFormServiceSec")]
    public interface IDestFormServiceSec {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://destinywireless.com/IDestFormServiceSec/SendData", ReplyAction="http://destinywireless.com/IDestFormServiceSec/SendDataResponse")]
        System.IAsyncResult BeginSendData(INKWRX_Mobile.FormService.DestInputMsg request, System.AsyncCallback callback, object asyncState);
        
        INKWRX_Mobile.FormService.DestRespMessage EndSendData(System.IAsyncResult result);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DestInputMsg", WrapperNamespace="http://destinywireless.com/")]
    public partial class DestInputMsg {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://destinywireless.com/", Order=0)]
        public string Date;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://destinywireless.com/", Order=1)]
        public string Data;
        
        public DestInputMsg() {
        }
        
        public DestInputMsg(string Date, string Data) {
            this.Date = Date;
            this.Data = Data;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DestRespMessage", WrapperNamespace="http://destinywireless.com/")]
    public partial class DestRespMessage {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://destinywireless.com/", Order=0)]
        public string Date;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://destinywireless.com/", Order=1)]
        public string Data;
        
        public DestRespMessage() {
        }
        
        public DestRespMessage(string Date, string Data) {
            this.Date = Date;
            this.Data = Data;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDestFormServiceSecChannel : INKWRX_Mobile.FormService.IDestFormServiceSec, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SendDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public SendDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Data {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DestFormServiceSecClient : System.ServiceModel.ClientBase<INKWRX_Mobile.FormService.IDestFormServiceSec>, INKWRX_Mobile.FormService.IDestFormServiceSec {
        
        private BeginOperationDelegate onBeginSendDataDelegate;
        
        private EndOperationDelegate onEndSendDataDelegate;
        
        private System.Threading.SendOrPostCallback onSendDataCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public DestFormServiceSecClient(EndpointConfiguration endpointConfiguration) : 
                base(DestFormServiceSecClient.GetBindingForEndpoint(endpointConfiguration), DestFormServiceSecClient.GetEndpointAddress(endpointConfiguration)) {
        }
        
        public DestFormServiceSecClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(DestFormServiceSecClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) {
        }
        
        public DestFormServiceSecClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(DestFormServiceSecClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress) {
        }
        
        public DestFormServiceSecClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<SendDataCompletedEventArgs> SendDataCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult INKWRX_Mobile.FormService.IDestFormServiceSec.BeginSendData(INKWRX_Mobile.FormService.DestInputMsg request, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSendData(request, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private System.IAsyncResult BeginSendData(string Date, string Data, System.AsyncCallback callback, object asyncState) {
            INKWRX_Mobile.FormService.DestInputMsg inValue = new INKWRX_Mobile.FormService.DestInputMsg();
            inValue.Date = Date;
            inValue.Data = Data;
            return ((INKWRX_Mobile.FormService.IDestFormServiceSec)(this)).BeginSendData(inValue, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        INKWRX_Mobile.FormService.DestRespMessage INKWRX_Mobile.FormService.IDestFormServiceSec.EndSendData(System.IAsyncResult result) {
            return base.Channel.EndSendData(result);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private string EndSendData(System.IAsyncResult result, out string Data) {
            INKWRX_Mobile.FormService.DestRespMessage retVal = ((INKWRX_Mobile.FormService.IDestFormServiceSec)(this)).EndSendData(result);
            Data = retVal.Data;
            return retVal.Date;
        }
        
        private System.IAsyncResult OnBeginSendData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string Date = ((string)(inValues[0]));
            string Data = ((string)(inValues[1]));
            return this.BeginSendData(Date, Data, callback, asyncState);
        }
        
        private object[] OnEndSendData(System.IAsyncResult result) {
            string Data = this.GetDefaultValueForInitialization<string>();
            string retVal = this.EndSendData(result, out Data);
            return new object[] {
                    Data,
                    retVal};
        }
        
        private void OnSendDataCompleted(object state) {
            if ((this.SendDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendDataCompleted(this, new SendDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SendDataAsync(string Date, string Data) {
            this.SendDataAsync(Date, Data, null);
        }
        
        public void SendDataAsync(string Date, string Data, object userState) {
            if ((this.onBeginSendDataDelegate == null)) {
                this.onBeginSendDataDelegate = new BeginOperationDelegate(this.OnBeginSendData);
            }
            if ((this.onEndSendDataDelegate == null)) {
                this.onEndSendDataDelegate = new EndOperationDelegate(this.OnEndSendData);
            }
            if ((this.onSendDataCompletedDelegate == null)) {
                this.onSendDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSendDataCompleted);
            }
            base.InvokeAsync(this.onBeginSendDataDelegate, new object[] {
                        Date,
                        Data}, this.onEndSendDataDelegate, this.onSendDataCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override INKWRX_Mobile.FormService.IDestFormServiceSec CreateChannel() {
            return new DestFormServiceSecClientChannel(this);
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IDestFormServiceSec)) {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IDestFormServiceSec1)) {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IDestFormServiceSec)) {
                return new System.ServiceModel.EndpointAddress("http://prelivedb.destinywireless.local/formmanagersec/service/DestFormServiceSec." +
                        "svc");
            }
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IDestFormServiceSec1)) {
                return new System.ServiceModel.EndpointAddress("https://prelivedb.destinywireless.local/formmanagersec/service/DestFormServiceSec" +
                        ".svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private class DestFormServiceSecClientChannel : ChannelBase<INKWRX_Mobile.FormService.IDestFormServiceSec>, INKWRX_Mobile.FormService.IDestFormServiceSec {
            
            public DestFormServiceSecClientChannel(System.ServiceModel.ClientBase<INKWRX_Mobile.FormService.IDestFormServiceSec> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginSendData(INKWRX_Mobile.FormService.DestInputMsg request, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = request;
                System.IAsyncResult _result = base.BeginInvoke("SendData", _args, callback, asyncState);
                return _result;
            }
            
            public INKWRX_Mobile.FormService.DestRespMessage EndSendData(System.IAsyncResult result) {
                object[] _args = new object[0];
                INKWRX_Mobile.FormService.DestRespMessage _result = ((INKWRX_Mobile.FormService.DestRespMessage)(base.EndInvoke("SendData", _args, result)));
                return _result;
            }
        }
        
        public enum EndpointConfiguration {
            
            WSHttpBinding_IDestFormServiceSec,
            
            WSHttpBinding_IDestFormServiceSec1,
        }
    }
}
