import { diffLines } from 'diff';

export interface DiffLine {
  value: string;
  added?: boolean;
  removed?: boolean;
}

export function createDiff(oldText: string, newText: string): DiffLine[] {
  return diffLines(oldText, newText);
}