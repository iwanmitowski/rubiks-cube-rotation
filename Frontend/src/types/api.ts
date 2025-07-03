export const Colour = {
  White: 0,
  Yellow: 1,
  Red: 2,
  Orange: 3,
  Blue: 4,
  Green: 5,
} as const;

export type Colour = typeof Colour[keyof typeof Colour];

export const Face = {
  Up: 0,
  Down: 1,
  Front: 2,
  Back: 3,
  Right: 4,
  Left: 5,
} as const;
export type Face = typeof Face[keyof typeof Face];

export interface MoveRequest {
  face: Face;
  clockwise: boolean;
}

export type ExplodedView = (Colour | null)[][];