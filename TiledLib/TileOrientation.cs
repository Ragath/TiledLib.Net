namespace TiledLib;

public enum TileOrientation : uint
{
    Default = 0b00000000_00000000_00000000_00000000,
    FlippedH = 0b10000000_00000000_00000000_00000000,
    FlippedV = 0b01000000_00000000_00000000_00000000,
    FlippedAD = 0b00100000_00000000_00000000_00000000,
    Rotate90CW = FlippedAD | FlippedH,
    Rotate180CCW = FlippedAD | FlippedH | FlippedV,
    Rotate270CCW = FlippedAD | FlippedV,
    MaskFlip = FlippedAD | FlippedH | FlippedV,
    MaskID = ~MaskFlip
}
