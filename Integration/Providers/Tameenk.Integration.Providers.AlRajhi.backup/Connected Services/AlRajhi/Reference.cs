﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tameenk.Integration.Providers.AlRajhi.AlRajhi {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AlRajhi.ITameenK")]
    public interface ITameenK {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/Quotation", ReplyAction="http://tempuri.org/ITameenK/QuotationResponse")]
        string Quotation([System.ServiceModel.MessageParameterAttribute(Name="Quotation")] string Quotation1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/Quotation", ReplyAction="http://tempuri.org/ITameenK/QuotationResponse")]
        System.Threading.Tasks.Task<string> QuotationAsync(string Quotation);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/Policy", ReplyAction="http://tempuri.org/ITameenK/PolicyResponse")]
        string Policy([System.ServiceModel.MessageParameterAttribute(Name="Policy")] string Policy1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/Policy", ReplyAction="http://tempuri.org/ITameenK/PolicyResponse")]
        System.Threading.Tasks.Task<string> PolicyAsync(string Policy);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/PolicySchedule", ReplyAction="http://tempuri.org/ITameenK/PolicyScheduleResponse")]
        string PolicySchedule([System.ServiceModel.MessageParameterAttribute(Name="PolicySchedule")] string PolicySchedule1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/PolicySchedule", ReplyAction="http://tempuri.org/ITameenK/PolicyScheduleResponse")]
        System.Threading.Tasks.Task<string> PolicyScheduleAsync(string PolicySchedule);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/VehicleImagesUpload", ReplyAction="http://tempuri.org/ITameenK/VehicleImagesUploadResponse")]
        string VehicleImagesUpload([System.ServiceModel.MessageParameterAttribute(Name="VehicleImagesUpload")] string VehicleImagesUpload1);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITameenK/VehicleImagesUpload", ReplyAction="http://tempuri.org/ITameenK/VehicleImagesUploadResponse")]
        System.Threading.Tasks.Task<string> VehicleImagesUploadAsync(string VehicleImagesUpload);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITameenKChannel : Tameenk.Integration.Providers.AlRajhi.AlRajhi.ITameenK, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TameenKClient : System.ServiceModel.ClientBase<Tameenk.Integration.Providers.AlRajhi.AlRajhi.ITameenK>, Tameenk.Integration.Providers.AlRajhi.AlRajhi.ITameenK {
        
        public TameenKClient() {
        }
        
        public TameenKClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TameenKClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TameenKClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TameenKClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Quotation(string Quotation1) {
            return base.Channel.Quotation(Quotation1);
        }
        
        public System.Threading.Tasks.Task<string> QuotationAsync(string Quotation) {
            return base.Channel.QuotationAsync(Quotation);
        }
        
        public string Policy(string Policy1) {
            return base.Channel.Policy(Policy1);
        }
        
        public System.Threading.Tasks.Task<string> PolicyAsync(string Policy) {
            return base.Channel.PolicyAsync(Policy);
        }
        
        public string PolicySchedule(string PolicySchedule1) {
            return base.Channel.PolicySchedule(PolicySchedule1);
        }
        
        public System.Threading.Tasks.Task<string> PolicyScheduleAsync(string PolicySchedule) {
            return base.Channel.PolicyScheduleAsync(PolicySchedule);
        }
        
        public string VehicleImagesUpload(string VehicleImagesUpload1) {
            return base.Channel.VehicleImagesUpload(VehicleImagesUpload1);
        }
        
        public System.Threading.Tasks.Task<string> VehicleImagesUploadAsync(string VehicleImagesUpload) {
            return base.Channel.VehicleImagesUploadAsync(VehicleImagesUpload);
        }
    }
}