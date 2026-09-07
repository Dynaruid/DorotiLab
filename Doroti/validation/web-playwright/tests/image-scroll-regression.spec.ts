import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';
import { PNG } from 'pngjs';

for (const suffix of ['', ' @dpr']) test(`image demo paints after lazy entry and retains fit across idle scroll${suffix}`, async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({width:1280,height:1000});
  await openDoroti(page,'&dorotiTestbedMode=sample');
  const extract=page.getByRole('button',{name:'Extract colors',exact:true});
  const contain=page.getByRole('radio',{name:'Contain',exact:true});
  const cover=page.getByRole('radio',{name:'Cover',exact:true});
  for(let i=0;i<60;i++) {
    const b=await extract.count()?await extract.boundingBox():null;
    if(b&&b.y>400&&b.y+b.height<990)break;
    await page.mouse.move(1255,700);await page.mouse.wheel(0,300);await page.waitForTimeout(350);
  }
  await expect(extract).toBeAttached();
  const bounds=(await contain.boundingBox())!;
  const left=(await extract.boundingBox())!.x;
  const crop={x:(left+1270)/2-60,y:bounds.y+bounds.height+25,width:120,height:180};
  await expect.poll(async()=>{
    const png=PNG.sync.read(await page.screenshot({clip:crop}));
    const colors=new Set<number>();
    for(let i=0;i<png.data.length;i+=16)colors.add(png.data.readUInt32BE(i));
    return colors.size;
  },{message:'The photo region has decoded image pixels, not an empty loading box',timeout:60000}).toBeGreaterThan(200);
  await page.screenshot({path:testInfo.outputPath('image-visible.png')});
  await expect(page.getByLabel(/Primary.*RGB/)).toHaveCount(0);
  const before=await page.screenshot({clip:crop});
  const b=(await cover.boundingBox())!;
  await page.mouse.click(b.x+b.width/2,b.y+b.height/2);
  await expect(cover).toHaveAttribute('aria-checked','true');
  await page.waitForTimeout(800);
  expect(before.equals(await page.screenshot({clip:crop}))).toBe(false);
  await page.mouse.move(1255,700);await page.mouse.wheel(0,-40);await page.waitForTimeout(5000);
  await page.mouse.wheel(0,40);await page.waitForTimeout(700);
  await expect(cover).toHaveAttribute('aria-checked','true');
  expect(runtimeErrors).toEqual([]);
});
