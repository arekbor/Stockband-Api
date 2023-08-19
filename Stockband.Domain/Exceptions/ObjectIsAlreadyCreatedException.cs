namespace Stockband.Domain.Exceptions;

public class ObjectAlreadyCreatedException:Exception
{
    public ObjectAlreadyCreatedException(Type model)
        :base ($"{model.Name} is already created")
    {
        
    }

    public ObjectAlreadyCreatedException(Type model, int id)
        :base($"{model.Name} {id} is already created")
    {
        
    }
    
    public ObjectAlreadyCreatedException(Type model, string name)
        :base($"{model.Name} {name} is already created")
    {
        
    }
}