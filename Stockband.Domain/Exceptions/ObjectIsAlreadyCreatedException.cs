namespace Stockband.Domain.Exceptions;

public class ObjectIsAlreadyCreatedException:Exception
{
    public ObjectIsAlreadyCreatedException(Type model)
        :base ($"{model.Name} is already created")
    {
        
    }

    public ObjectIsAlreadyCreatedException(Type model, int id)
        :base($"{model.Name} {id} is already created")
    {
        
    }
    
    public ObjectIsAlreadyCreatedException(Type model, string name)
        :base($"{model.Name} {name} is already created")
    {
        
    }
}