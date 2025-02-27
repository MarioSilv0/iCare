import { MapKeysPipe } from './map-keys-pipe';

describe('MapKeysPipe', () => {
  let pipe: MapKeysPipe;

  beforeEach(() => {
    pipe = new MapKeysPipe();
  });

  it('should create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should return an empty array for an empty map', () => {
    const map = new Map();
    expect(pipe.transform(map)).toEqual([]);
  });

  it('should return an array of keys from the map', () => {
    const map = new Map([
      ['key1', 'value1'],
      ['key2', 'value2'],
      ['key3', 'value3'],
    ]);
    expect(pipe.transform(map)).toEqual(['key1', 'key2', 'key3']);
  });

  it('should handle numbers as keys', () => {
    const map = new Map([
      [1, 'one'],
      [2, 'two'],
      [3, 'three'],
    ]);
    expect(pipe.transform(map)).toEqual([1, 2, 3]);
  });

  it('should handle objects as keys', () => {
    const key1 = { id: 1 };
    const key2 = { id: 2 };
    const map = new Map([
      [key1, 'value1'],
      [key2, 'value2'],
    ]);
    expect(pipe.transform(map)).toEqual([key1, key2]);
  });
});
