import React, { useRef, useEffect } from "react";
import * as THREE from "three";
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls.js";
import { Colour } from "../types/api";

interface CubeViewerProps {
  view: (Colour | null)[][]; // 9x12 net layout
  exploded: boolean;
}

const CUBELET_SIZE = 1;
const GAP = 0.1;
const BLACK_MATERIAL = new THREE.MeshBasicMaterial({ color: 0x1a1a1a });

const COLOUR_MAP: { [key in Colour]: THREE.MeshBasicMaterial } = {
  [Colour.White]: new THREE.MeshBasicMaterial({ color: 0xffffff }),
  [Colour.Yellow]: new THREE.MeshBasicMaterial({ color: 0xffff00 }),
  [Colour.Red]: new THREE.MeshBasicMaterial({ color: 0xb71c1c }),
  [Colour.Orange]: new THREE.MeshBasicMaterial({ color: 0xff6f00 }),
  [Colour.Blue]: new THREE.MeshBasicMaterial({ color: 0x0d47a1 }),
  [Colour.Green]: new THREE.MeshBasicMaterial({ color: 0x2e7d32 }),
};

const FACE_DEFINITIONS = {
  Right: { normal: new THREE.Vector3(1, 0, 0), materialIndex: 0 }, // +X
  Left: { normal: new THREE.Vector3(-1, 0, 0), materialIndex: 1 }, // -X
  Up: { normal: new THREE.Vector3(0, 1, 0), materialIndex: 2 }, // +Y
  Down: { normal: new THREE.Vector3(0, -1, 0), materialIndex: 3 }, // -Y
  Front: { normal: new THREE.Vector3(0, 0, 1), materialIndex: 4 }, // +Z
  Back: { normal: new THREE.Vector3(0, 0, -1), materialIndex: 5 }, // -Z
};

type FaceName = keyof typeof FACE_DEFINITIONS;

interface Cubelet {
  position: THREE.Vector3;
  colors: Map<FaceName, Colour>;
}

export const CubeViewer: React.FC<CubeViewerProps> = ({ view, exploded }) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xeeeeee);
    const renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
    const meshes: THREE.Mesh[] = [];

    const updateSize = () => {
      const { clientWidth, clientHeight } = canvas.parentElement || canvas;
      renderer.setSize(clientWidth, clientHeight);
      if ("aspect" in camera) {
        camera.aspect = clientWidth / clientHeight;
        camera.updateProjectionMatrix();
      }
    };

    let camera: THREE.PerspectiveCamera | THREE.OrthographicCamera;
    let controls: OrbitControls | null = null;

    if (exploded) {
      const { camera: orthoCamera, controls: orthoControls } =
        setupExplodedView(canvas);
      camera = orthoCamera;
      controls = orthoControls;
      renderExplodedNet(scene, view, meshes);
    } else {
      const { camera: perspectiveCamera, controls: perspectiveControls } =
        setup3DView(canvas);
      camera = perspectiveCamera;
      controls = perspectiveControls;
      render3DCube(scene, view, meshes);
    }

    updateSize();

    window.addEventListener("resize", updateSize);

    const animate = () => {
      if (!renderer.domElement) return; // Stop if renderer is disposed
      controls?.update();
      renderer.render(scene, camera);
      requestAnimationFrame(animate);
    };
    animate();

    return () => {
      window.removeEventListener("resize", updateSize);
      controls?.dispose();
      meshes.forEach((m) => {
        scene.remove(m);
        m.geometry.dispose();
      });
      Object.values(COLOUR_MAP).forEach((mat) => mat.dispose());
      BLACK_MATERIAL.dispose();
      renderer.dispose();
      scene.clear();
    };
  }, [view, exploded]);

  return (
    <canvas
      ref={canvasRef}
      style={{ width: "100%", height: "100%", display: "block" }}
    />
  );
};

/**
 * Sets up the 3D view camera and controls.
 * Uses a perspective camera for a 3D cube view.
 */
function setup3DView(canvas: HTMLCanvasElement) {
  const camera = new THREE.PerspectiveCamera(
    50,
    canvas.clientWidth / canvas.clientHeight,
    0.1,
    1000
  );
  camera.position.set(4, 5, 6);
  camera.lookAt(0, 0, 0);

  const controls = new OrbitControls(camera, canvas);
  controls.enableDamping = true;
  controls.dampingFactor = 0.1;

  return { camera, controls };
}

/**
 * Sets up the exploded view camera and controls.
 * Uses an orthographic camera for a 2D grid layout.
 */
function setupExplodedView(canvas: HTMLCanvasElement) {
  const aspect = canvas.clientWidth / canvas.clientHeight;
  const frustumSize = 10;
  const camera = new THREE.OrthographicCamera(
    (frustumSize * aspect) / -2,
    (frustumSize * aspect) / 2,
    frustumSize / 2,
    frustumSize / -2,
    0.1,
    100
  );
  camera.position.set(0, 0, 10);
  camera.lookAt(0, 0, 0);

  const controls = new OrbitControls(camera, canvas);
  controls.enableRotate = false;
  controls.enableDamping = true;

  return { camera, controls };
}

/**
 * Renders the 9x12 exploded net as a 2D grid.
 */
function renderExplodedNet(
  scene: THREE.Scene,
  view: (Colour | null)[][],
  meshes: THREE.Mesh[]
) {
  const planeGeom = new THREE.PlaneGeometry(CUBELET_SIZE, CUBELET_SIZE);
  const gridWidth = 12;
  const gridHeight = 9;

  view.forEach((row, rowIdx) => {
    row.forEach((colour, colIdx) => {
      if (colour === null) return;
      const material = COLOUR_MAP[colour] || BLACK_MATERIAL;
      const sticker = new THREE.Mesh(planeGeom, material);
      const x = (colIdx - (gridWidth - 1) / 2) * (CUBELET_SIZE + GAP);
      const y = -(rowIdx - (gridHeight - 1) / 2) * (CUBELET_SIZE + GAP);
      sticker.position.set(x, y, 0);
      scene.add(sticker);
      meshes.push(sticker);
    });
  });
}

/**
 * Reconstructs the 3x3x3 cube from the flat net data and renders it.
 */
function render3DCube(
  scene: THREE.Scene,
  view: (Colour | null)[][],
  meshes: THREE.Mesh[]
) {
  const cubelets = new Map<string, Cubelet>();

  view.forEach((row, rowIdx) => {
    row.forEach((colour, colIdx) => {
      if (colour === null) return;
      const info = getCubeletInfoFromNet(rowIdx, colIdx);
      if (!info) return;
      const { position, faceName } = info;
      const key = `${position.x},${position.y},${position.z}`;
      if (!cubelets.has(key)) {
        cubelets.set(key, { position, colors: new Map() });
      }
      cubelets.get(key)!.colors.set(faceName, colour);
    });
  });

  const cubeletGeom = new THREE.BoxGeometry(
    CUBELET_SIZE,
    CUBELET_SIZE,
    CUBELET_SIZE
  );

  cubelets.forEach((cubelet) => {
    const materials = Array(6).fill(BLACK_MATERIAL);
    cubelet.colors.forEach((colour, faceName) => {
      const { materialIndex } = FACE_DEFINITIONS[faceName];
      materials[materialIndex] = COLOUR_MAP[colour];
    });

    const mesh = new THREE.Mesh(cubeletGeom, materials);

    mesh.position.copy(cubelet.position).multiplyScalar(CUBELET_SIZE + GAP);

    scene.add(mesh);
    meshes.push(mesh);
  });
}

/**
 * A mathematical mapping from the 2D net's [row, col] to a 3D cubelet's
 * position and face orientation. This is the core of the reconstruction.
 *
 * Cubelet coordinates are in {-1, 0, 1} for x, y, z.
 */
function getCubeletInfoFromNet(
  rowIdx: number,
  colIdx: number
): { position: THREE.Vector3; faceName: FaceName } | null {
  let faceName: FaceName | null = null;
  let localRow = 0,
    localCol = 0;

  if (rowIdx <= 2 && colIdx >= 3 && colIdx <= 5) {
    faceName = "Up";
    localRow = rowIdx;
    localCol = colIdx - 3;
  } else if (rowIdx >= 3 && rowIdx <= 5) {
    if (colIdx <= 2) {
      faceName = "Left";
      localRow = rowIdx - 3;
      localCol = colIdx;
    } else if (colIdx <= 5) {
      faceName = "Front";
      localRow = rowIdx - 3;
      localCol = colIdx - 3;
    } else if (colIdx <= 8) {
      faceName = "Right";
      localRow = rowIdx - 3;
      localCol = colIdx - 6;
    } else if (colIdx <= 11) {
      faceName = "Back";
      localRow = rowIdx - 3;
      localCol = colIdx - 9;
    }
  } else if (rowIdx >= 6 && colIdx <= 8 && colIdx >= 3 && colIdx <= 5) {
    faceName = "Down";
    localRow = rowIdx - 6;
    localCol = colIdx - 3;
  } else {
    return null;
  }

  const u = localCol - 1; // local x-coord
  const v = localRow - 1; // local y-coord

  let position: THREE.Vector3;

  // This set of transformations correctly "folds" the flat, un-rotated net
  // data into a 3D cube based on the standard L-F-R-B layout.
  switch (faceName) {
    case "Up":
      position = new THREE.Vector3(u, 1, v);
      break;
    case "Down":
      position = new THREE.Vector3(u, -1, -v);
      break;
    case "Left":
      position = new THREE.Vector3(-1, -v, u);
      break;
    case "Right":
      position = new THREE.Vector3(1, -v, -u);
      break;
    case "Front":
      position = new THREE.Vector3(u, -v, 1);
      break;
    case "Back":
      position = new THREE.Vector3(-u, -v, -1);
      break;
    default:
      return null;
  }

  return { position, faceName };
}
