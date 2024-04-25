namespace ShuttleUI.Gallery.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
