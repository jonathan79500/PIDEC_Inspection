import { IsActiveForUserPipe } from './is-active-for-user.pipe';

describe('IsActiveForUserPipe', () => {
  it('create an instance', () => {
    const pipe = new IsActiveForUserPipe();
    expect(pipe).toBeTruthy();
  });
});
