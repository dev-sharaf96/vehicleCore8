﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tameenk.Integration.Providers.Ahlia.AhliaService {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.tameenak.technoSys.com/")]
    public partial class AIMSFault : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string faultStringField;
        
        private string messageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string faultString {
            get {
                return this.faultStringField;
            }
            set {
                this.faultStringField = value;
                this.RaisePropertyChanged("faultString");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("message");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.tameenak.technoSys.com/")]
    public partial class polResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private byte[] policyResFileField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", Order=0)]
        public byte[] policyResFile {
            get {
                return this.policyResFileField;
            }
            set {
                this.policyResFileField = value;
                this.RaisePropertyChanged("policyResFile");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.tameenak.technoSys.com/")]
    public partial class policyRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string authenticationField;
        
        private byte[] policyReqFileField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string authentication {
            get {
                return this.authenticationField;
            }
            set {
                this.authenticationField = value;
                this.RaisePropertyChanged("authentication");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", Order=1)]
        public byte[] policyReqFile {
            get {
                return this.policyReqFileField;
            }
            set {
                this.policyReqFileField = value;
                this.RaisePropertyChanged("policyReqFile");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://service.tameenak.technoSys.com/", ConfigurationName="AhliaService.TameenakImpl")]
    public interface TameenakImpl {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://service.tameenak.technoSys.com/TameenakImpl/PolicyRequest", ReplyAction="http://service.tameenak.technoSys.com/TameenakImpl/PolicyResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Tameenk.Integration.Providers.Ahlia.AhliaService.AIMSFault), Action="http://service.tameenak.technoSys.com/TameenakImpl/Policy/Fault/AIMSFault", Name="AIMSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse Policy(Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://service.tameenak.technoSys.com/TameenakImpl/PolicyRequest", ReplyAction="http://service.tameenak.technoSys.com/TameenakImpl/PolicyResponse")]
        System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse> PolicyAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://service.tameenak.technoSys.com/TameenakImpl/QuotationRequest", ReplyAction="http://service.tameenak.technoSys.com/TameenakImpl/QuotationResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Tameenk.Integration.Providers.Ahlia.AhliaService.AIMSFault), Action="http://service.tameenak.technoSys.com/TameenakImpl/Quotation/Fault/AIMSFault", Name="AIMSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse Quotation(Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://service.tameenak.technoSys.com/TameenakImpl/QuotationRequest", ReplyAction="http://service.tameenak.technoSys.com/TameenakImpl/QuotationResponse")]
        System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse> QuotationAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Policy", WrapperNamespace="http://service.tameenak.technoSys.com/", IsWrapped=true)]
    public partial class PolicyRequest1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.tameenak.technoSys.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Tameenk.Integration.Providers.Ahlia.AhliaService.policyRequest PolicyRequest;
        
        public PolicyRequest1() {
        }
        
        public PolicyRequest1(Tameenk.Integration.Providers.Ahlia.AhliaService.policyRequest PolicyRequest) {
            this.PolicyRequest = PolicyRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="PolicyResponse", WrapperNamespace="http://service.tameenak.technoSys.com/", IsWrapped=true)]
    public partial class PolicyResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.tameenak.technoSys.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Tameenk.Integration.Providers.Ahlia.AhliaService.polResponse @return;
        
        public PolicyResponse() {
        }
        
        public PolicyResponse(Tameenk.Integration.Providers.Ahlia.AhliaService.polResponse @return) {
            this.@return = @return;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.tameenak.technoSys.com/")]
    public partial class quoteRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string authenticationField;
        
        private byte[] quoteFileField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string authentication {
            get {
                return this.authenticationField;
            }
            set {
                this.authenticationField = value;
                this.RaisePropertyChanged("authentication");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", Order=1)]
        public byte[] quoteFile {
            get {
                return this.quoteFileField;
            }
            set {
                this.quoteFileField = value;
                this.RaisePropertyChanged("quoteFile");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.tameenak.technoSys.com/")]
    public partial class quoteResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private byte[] resFileField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", Order=0)]
        public byte[] resFile {
            get {
                return this.resFileField;
            }
            set {
                this.resFileField = value;
                this.RaisePropertyChanged("resFile");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Quotation", WrapperNamespace="http://service.tameenak.technoSys.com/", IsWrapped=true)]
    public partial class QuotationRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.tameenak.technoSys.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Tameenk.Integration.Providers.Ahlia.AhliaService.quoteRequest QuoteRequest;
        
        public QuotationRequest() {
        }
        
        public QuotationRequest(Tameenk.Integration.Providers.Ahlia.AhliaService.quoteRequest QuoteRequest) {
            this.QuoteRequest = QuoteRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="QuotationResponse", WrapperNamespace="http://service.tameenak.technoSys.com/", IsWrapped=true)]
    public partial class QuotationResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.tameenak.technoSys.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Tameenk.Integration.Providers.Ahlia.AhliaService.quoteResponse @return;
        
        public QuotationResponse() {
        }
        
        public QuotationResponse(Tameenk.Integration.Providers.Ahlia.AhliaService.quoteResponse @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TameenakImplChannel : Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TameenakImplClient : System.ServiceModel.ClientBase<Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl>, Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl {
        
        public TameenakImplClient() {
        }
        
        public TameenakImplClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TameenakImplClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TameenakImplClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TameenakImplClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl.Policy(Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 request) {
            return base.Channel.Policy(request);
        }
        
        public Tameenk.Integration.Providers.Ahlia.AhliaService.polResponse Policy(Tameenk.Integration.Providers.Ahlia.AhliaService.policyRequest PolicyRequest) {
            Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 inValue = new Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1();
            inValue.PolicyRequest = PolicyRequest;
            Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse retVal = ((Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl)(this)).Policy(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse> Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl.PolicyAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 request) {
            return base.Channel.PolicyAsync(request);
        }
        
        public System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyResponse> PolicyAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.policyRequest PolicyRequest) {
            Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1 inValue = new Tameenk.Integration.Providers.Ahlia.AhliaService.PolicyRequest1();
            inValue.PolicyRequest = PolicyRequest;
            return ((Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl)(this)).PolicyAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl.Quotation(Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest request) {
            return base.Channel.Quotation(request);
        }
        
        public Tameenk.Integration.Providers.Ahlia.AhliaService.quoteResponse Quotation(Tameenk.Integration.Providers.Ahlia.AhliaService.quoteRequest QuoteRequest) {
            Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest inValue = new Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest();
            inValue.QuoteRequest = QuoteRequest;
            Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse retVal = ((Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl)(this)).Quotation(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse> Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl.QuotationAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest request) {
            return base.Channel.QuotationAsync(request);
        }
        
        public System.Threading.Tasks.Task<Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationResponse> QuotationAsync(Tameenk.Integration.Providers.Ahlia.AhliaService.quoteRequest QuoteRequest) {
            Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest inValue = new Tameenk.Integration.Providers.Ahlia.AhliaService.QuotationRequest();
            inValue.QuoteRequest = QuoteRequest;
            return ((Tameenk.Integration.Providers.Ahlia.AhliaService.TameenakImpl)(this)).QuotationAsync(inValue);
        }
    }
}
