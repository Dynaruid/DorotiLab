import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';

test('parked right sections survive a narrow theme change and move on demand', async ({page,runtimeErrors})=>{
 await page.setViewportSize({width:1280,height:900});
 await openDoroti(page,'&dorotiTestbedMode=sample&dorotiSectionViewport=indexed');
 const choice=page.getByRole('checkbox',{name:'Option 2',exact:true});
 async function reveal(x:number) {
  for(let step=0;step<35;step++) {
   const b=await choice.count()?await choice.boundingBox():null;
   if(b&&b.y>140&&b.y+b.height<760) {await page.waitForTimeout(500);return;}
   await page.mouse.move(x,600);await page.mouse.wheel(0,b&&b.y<140?-220:220);await page.waitForTimeout(400);
  }
  throw Error('Option 2 checkbox was not materialized');
 }
 await reveal(1000);
 await expect(choice).toHaveAttribute('aria-checked','mixed');
 let box=(await choice.boundingBox())!;
 await page.mouse.click(box.x+box.width/2,box.y+box.height/2);
 await expect(choice).toHaveAttribute('aria-checked','false');
 await page.setViewportSize({width:800,height:900});await page.waitForTimeout(1400);
 // Left remained at Actions; the right section must be excluded while parked.
 await expect(choice).toHaveCount(0);
 const brightness=page.getByRole('button').and(page.locator('[aria-description="Toggle brightness"]'));
 box=(await brightness.boundingBox())!;
 await page.mouse.click(box.x+box.width/2,box.y+box.height/2);await page.waitForTimeout(600);
 await expect(choice).toHaveCount(0);
 await reveal(500);
 await expect(choice).toHaveAttribute('aria-checked','false');
 await page.setViewportSize({width:1280,height:900});await page.waitForTimeout(1400);
 await reveal(1000);
 await expect(choice).toHaveCount(1);
 await expect(choice).toHaveAttribute('aria-checked','false');
 expect(runtimeErrors).toEqual([]);
});
