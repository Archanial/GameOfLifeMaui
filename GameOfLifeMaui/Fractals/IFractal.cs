namespace GameOfLifeMaui.Fractals;

public abstract class Fractal
{
    protected int? Width;

    protected int? Height;

    protected int? Steps;
    
    public abstract void Act();

    public abstract void SetParameters(int? width, int? height, int? steps);
}