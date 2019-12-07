namespace GraphicsServices.GraphicObjTypes
{
    interface IGraphicObject
    {
        int RequiredDataLength { get; }
        string Prefix { get; }

        void ParseFromString(string[] data);
    }
}
