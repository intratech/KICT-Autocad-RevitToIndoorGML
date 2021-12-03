import * as THREE from 'three';

export class MaterialManager {
  public static FLOOR_MATERIAL = new THREE.MeshLambertMaterial({
    color: 0x808080,
    side: THREE.DoubleSide
  });
  public static CLOSURE_SURFACE_MATERIAL = new THREE.MeshLambertMaterial({
    color: 0xf5cece,
    side: THREE.DoubleSide,
    transparent: true,
    opacity: 0.7
  });
  public static LINE_MATERIAL = new THREE.LineBasicMaterial({
    color: 0xff0000
  });
  public static LINE_MATERIAL2 = new THREE.LineBasicMaterial({
    color: 0x0000ff
  });
  public static SKY_MATERIAL = new THREE.MeshLambertMaterial({
    color: 0x808080,
    side: THREE.BackSide
  });
}
