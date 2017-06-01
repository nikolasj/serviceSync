﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1.ServiceReference1 {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/WcfService2")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
                }
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IService1")]
    public interface IService1 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetData", ReplyAction="http://tempuri.org/IService1/GetDataResponse")]
        string GetData(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetData", ReplyAction="http://tempuri.org/IService1/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IService1/GetDataUsingDataContractResponse")]
        ConsoleApplication1.ServiceReference1.CompositeType GetDataUsingDataContract(ConsoleApplication1.ServiceReference1.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IService1/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<ConsoleApplication1.ServiceReference1.CompositeType> GetDataUsingDataContractAsync(ConsoleApplication1.ServiceReference1.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncCitiesKudaGo", ReplyAction="http://tempuri.org/IService1/SyncCitiesKudaGoResponse")]
        bool SyncCitiesKudaGo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncCitiesKudaGo", ReplyAction="http://tempuri.org/IService1/SyncCitiesKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncCitiesKudaGoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncCitiesCulture", ReplyAction="http://tempuri.org/IService1/SyncCitiesCultureResponse")]
        bool SyncCitiesCulture(string city);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncCitiesCulture", ReplyAction="http://tempuri.org/IService1/SyncCitiesCultureResponse")]
        System.Threading.Tasks.Task<bool> SyncCitiesCultureAsync(string city);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncEventsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncEventsKudaGoResponse")]
        bool SyncEventsKudaGo(System.DateTime eventDate, int locationId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncEventsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncEventsKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncEventsKudaGoAsync(System.DateTime eventDate, int locationId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncPlacesKudaGo", ReplyAction="http://tempuri.org/IService1/SyncPlacesKudaGoResponse")]
        bool SyncPlacesKudaGo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncPlacesKudaGo", ReplyAction="http://tempuri.org/IService1/SyncPlacesKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncPlacesKudaGoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncFilmsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncFilmsKudaGoResponse")]
        bool SyncFilmsKudaGo(System.DateTime dateFrom, System.DateTime dateTo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncFilmsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncFilmsKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncFilmsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncFilmDetailsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncFilmDetailsKudaGoResponse")]
        bool SyncFilmDetailsKudaGo(int Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncFilmDetailsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncFilmDetailsKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncFilmDetailsKudaGoAsync(int Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncShowFilmsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncShowFilmsKudaGoResponse")]
        bool SyncShowFilmsKudaGo(System.DateTime dateFrom, System.DateTime dateTo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncShowFilmsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncShowFilmsKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncShowFilmsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncShowsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncShowsKudaGoResponse")]
        bool SyncShowsKudaGo(System.DateTime dateFrom, System.DateTime dateTo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/SyncShowsKudaGo", ReplyAction="http://tempuri.org/IService1/SyncShowsKudaGoResponse")]
        System.Threading.Tasks.Task<bool> SyncShowsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService1Channel : ConsoleApplication1.ServiceReference1.IService1, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service1Client : System.ServiceModel.ClientBase<ConsoleApplication1.ServiceReference1.IService1>, ConsoleApplication1.ServiceReference1.IService1 {
        
        public Service1Client() {
        }
        
        public Service1Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service1Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetData(int value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(int value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public ConsoleApplication1.ServiceReference1.CompositeType GetDataUsingDataContract(ConsoleApplication1.ServiceReference1.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<ConsoleApplication1.ServiceReference1.CompositeType> GetDataUsingDataContractAsync(ConsoleApplication1.ServiceReference1.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
        
        public bool SyncCitiesKudaGo() {
            return base.Channel.SyncCitiesKudaGo();
        }
        
        public System.Threading.Tasks.Task<bool> SyncCitiesKudaGoAsync() {
            return base.Channel.SyncCitiesKudaGoAsync();
        }
        
        public bool SyncCitiesCulture(string city) {
            return base.Channel.SyncCitiesCulture(city);
        }
        
        public System.Threading.Tasks.Task<bool> SyncCitiesCultureAsync(string city) {
            return base.Channel.SyncCitiesCultureAsync(city);
        }
        
        public bool SyncEventsKudaGo(System.DateTime eventDate, int locationId) {
            return base.Channel.SyncEventsKudaGo(eventDate, locationId);
        }
        
        public System.Threading.Tasks.Task<bool> SyncEventsKudaGoAsync(System.DateTime eventDate, int locationId) {
            return base.Channel.SyncEventsKudaGoAsync(eventDate, locationId);
        }
        
        public bool SyncPlacesKudaGo() {
            return base.Channel.SyncPlacesKudaGo();
        }
        
        public System.Threading.Tasks.Task<bool> SyncPlacesKudaGoAsync() {
            return base.Channel.SyncPlacesKudaGoAsync();
        }
        
        public bool SyncFilmsKudaGo(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncFilmsKudaGo(dateFrom, dateTo);
        }
        
        public System.Threading.Tasks.Task<bool> SyncFilmsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncFilmsKudaGoAsync(dateFrom, dateTo);
        }
        
        public bool SyncFilmDetailsKudaGo(int Id) {
            return base.Channel.SyncFilmDetailsKudaGo(Id);
        }
        
        public System.Threading.Tasks.Task<bool> SyncFilmDetailsKudaGoAsync(int Id) {
            return base.Channel.SyncFilmDetailsKudaGoAsync(Id);
        }
        
        public bool SyncShowFilmsKudaGo(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncShowFilmsKudaGo(dateFrom, dateTo);
        }
        
        public System.Threading.Tasks.Task<bool> SyncShowFilmsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncShowFilmsKudaGoAsync(dateFrom, dateTo);
        }
        
        public bool SyncShowsKudaGo(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncShowsKudaGo(dateFrom, dateTo);
        }
        
        public System.Threading.Tasks.Task<bool> SyncShowsKudaGoAsync(System.DateTime dateFrom, System.DateTime dateTo) {
            return base.Channel.SyncShowsKudaGoAsync(dateFrom, dateTo);
        }
    }
}
