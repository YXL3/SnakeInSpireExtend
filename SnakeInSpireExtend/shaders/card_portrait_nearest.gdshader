shader_type canvas_item;

// The sampler hint enforces nearest-neighbor sampling independently of
// CanvasItem.texture_filter, so no filter state is written to pooled NCard nodes.
uniform sampler2D portrait_texture : filter_nearest, repeat_disable;

// Capture the CanvasItem modulate/self-modulate color in the vertex stage.
varying vec4 modulate_color;

void vertex() {
    modulate_color = COLOR;
}

void fragment() {
    COLOR = texture(portrait_texture, UV) * modulate_color;
}
