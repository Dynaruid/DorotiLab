import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';

test('sample selection survives responsive reparenting and theme updates', async ({ page, runtimeErrors }) => {
  await page.setViewportSize({width:1280,height:900});
  await openDoroti(page,'&dorotiTestbedMode=sample');
  const week=page.getByRole('radio',{name:'Week',exact:true});
  async function findWeek() {
    for(let i=0;i<40;i++) {
      const b=await week.count()?await week.boundingBox():null;
      if(b&&b.y>150&&b.y+b.height<780)return b;
      await page.mouse.move(500,600);await page.mouse.wheel(0,b&&b.y<150?-180:180);await page.waitForTimeout(400);
    }
    throw Error('Week control not visible');
  }
  let b=await findWeek();await page.mouse.click(b.x+b.width/2,b.y+b.height/2);
  await expect(week).toHaveAttribute('aria-checked','true');
  // A geometry-only semantics update must retain the native control object.
  await week.evaluate(e=>Object.assign(globalThis,{__retainedWeek:e}));
  await page.setViewportSize({width:1240,height:900});await page.waitForTimeout(1200);
  expect(await week.evaluate(e=>e===(globalThis as any).__retainedWeek)).toBe(true);
  for(const width of [800,1280]) {
    await page.setViewportSize({width,height:900});await page.waitForTimeout(1500);
    await findWeek();await expect(week).toHaveAttribute('aria-checked','true');
  }
  await page.mouse.move(20,40);await page.waitForTimeout(700);
  const before=await page.screenshot();
  const brightness=page.locator('[role="button"][aria-description="Toggle brightness"]');
  const toggle=(await brightness.boundingBox())!;
  await page.mouse.click(toggle.x+toggle.width/2,toggle.y+toggle.height/2);
  await page.mouse.move(20,40);await page.waitForTimeout(1500);
  expect(before.equals(await page.screenshot()),'Theme change reaches retained content').toBe(false);
  await findWeek();await expect(week).toHaveAttribute('aria-checked','true');
  b=await findWeek();await page.mouse.click(b.x+b.width/2,b.y+b.height/2);
  await expect(week).toHaveAttribute('aria-checked','true'); // Single choice remains selected.
  expect(runtimeErrors).toEqual([]);
});
