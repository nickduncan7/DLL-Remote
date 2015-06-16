using System.Collections.Generic;
using System.ServiceModel;
using CommonTypes;

namespace DLLRemoteService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    [ServiceKnownType(typeof (Library))]
    [ServiceKnownType(typeof (Method))]
    [ServiceKnownType(typeof (Parameter))]
    public interface IWebService
    {
        [OperationContract]
        List<string> GetRegisteredDLLs();

        [OperationContract]
        List<string> GetDLLMethods(string path);

        [OperationContract]
        Library GetLibraryInformation(string path);

        [OperationContract]
        string ExecuteMethod(string path, string method, List<object> parameters);
    }
}