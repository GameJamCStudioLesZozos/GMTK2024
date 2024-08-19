using Godot;
using System;

public partial class CollisionShape2dAutoFill : CollisionPolygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Polygon2D polygon = GetNodeOrNull<Polygon2D>("%GroundPolygon");
        if (polygon is null)
			polygon = GetParent().GetParent<Polygon2D>();

		if (polygon != null)
			Polygon = polygon.Polygon;
	}
}
