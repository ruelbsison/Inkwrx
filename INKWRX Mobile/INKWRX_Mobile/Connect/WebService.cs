using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using INKWRX_Mobile.FormService;
using INKWRX_Mobile.ServiceCenter;

namespace INKWRX_Mobile.Connect
{
    public class WebService
    {
        public WebService(string formUrl, string serviceUrl)
        {
            this.formServiceUrl = formUrl;
            this.serviceCenterUrl = serviceUrl;
        }

        private string formServiceUrl;
        private string serviceCenterUrl;

        public DestFormServiceSecClient GetFormServiceClient()
        {
            return new DestFormServiceSecClient(CreateBasicHttp(this.formServiceUrl.Contains("https://")), new EndpointAddress(new Uri(this.formServiceUrl), new AddressHeader[3]
            {
                AddressHeader.CreateAddressHeader(HeaderContentType, NameSpace, HeaderContentTypeValue),
                AddressHeader.CreateAddressHeader(HeaderAccept, NameSpace, HeaderContentTypeValue),
                AddressHeader.CreateAddressHeader(HeaderConnection, NameSpace, HeaderCloseValue)
            }));
        }

        public SvcCenterSecServiceClient GetServiceCenterClient()
        {
            return new SvcCenterSecServiceClient(CreateBasicHttp(this.serviceCenterUrl.Contains("https://")), new EndpointAddress(new Uri(this.serviceCenterUrl), new AddressHeader[3]
            {
                AddressHeader.CreateAddressHeader(HeaderContentType, NameSpace, HeaderContentTypeValue),
                AddressHeader.CreateAddressHeader(HeaderAccept, NameSpace, HeaderContentTypeValue),
                AddressHeader.CreateAddressHeader(HeaderConnection, NameSpace, HeaderCloseValue)
            }));
        }

        private static CustomBinding CreateBasicHttp(bool https = false)
        {
            var customBinding = new CustomBinding();
            customBinding.Namespace = NameSpace;
            customBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8));
            var bindingElement = https ? new HttpsTransportBindingElement() : new HttpTransportBindingElement();
            bindingElement.MaxReceivedMessageSize = 2147483647;
            bindingElement.MaxBufferSize = 2147483647;
            customBinding.Elements.Add(bindingElement);
            return customBinding;
        }

        private static readonly String NameSpace = "http://destinywireless.com/";
        private static readonly String HeaderContentType = "Content-Type";
        private static readonly String HeaderAccept = "Accept";
        private static readonly String HeaderConnection = "Connection";
        private static readonly String HeaderContentTypeValue = "application/soap+xml";
        private static readonly String HeaderCloseValue = "close";
    }
}
