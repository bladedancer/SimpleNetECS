using Components;

public static class NeuralExtensionMethods
{
    public static NetData Clone(this NetData net)
    {
        NetData clone = new NetData();
        clone.LayerSizes = new int[net.LayerSizes.Length];
        net.LayerSizes.CopyTo(clone.LayerSizes, 0);
        clone.Weights = new double[net.Weights.Length];
        net.Weights.CopyTo(clone.Weights, 0);
        return clone;
    }
}