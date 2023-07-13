namespace Stockband.Domain.Exceptions;

public class ObjectNotFound:Exception
{
    public ObjectNotFound(Type model)
        :base($"{model.Name} not found")
    {
        
    }

    public ObjectNotFound(Type model, object id)
        :base($"{model.Name} {id} not found")
    {
        
    }
    
    public ObjectNotFound(Type model, object id, object id2)
        :base($"{model.Name} {id} and {id2} not found")
    {
        
    }
    
    public ObjectNotFound(string objectName)
        :base($"{objectName} not found")
    {
        
    }
}