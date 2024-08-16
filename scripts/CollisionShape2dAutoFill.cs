using Godot;
using System;

public partial class CollisionShape2dAutoFill : CollisionPolygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Polygon2D polygon = GetNode<Polygon2D>("%GroundPolygon");
		if (polygon != null)
			Polygon = polygon.Polygon;
	}
}
