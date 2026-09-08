import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import { PNG } from 'pngjs';

for(const suffix of ['', ' @dpr']) test(`scrolled sample restores the right column after wide narrow wide${suffix}`, async ({page,runtimeErrors},testInfo)=>{
 await page.setViewportSize({width:1280,height:900});
 await openDoroti(page,'&dorotiTestbedMode=sample');
 async function resize(width: number) {
  await page.setViewportSize({width,height:900});
  await expect.poll(async () => {
   const frame = await captureDiagnostics(page);
   return frame.snapshot.logicalWidth === width && frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation;
  }, {message:'Resize has presented the requested viewport before scrolling'}).toBe(true);
 }
 for(const x of [600,1000]) {
  await page.mouse.move(x,700);
  for(let i=0;i<15;i++){await page.mouse.wheel(0,500);await page.waitForTimeout(150);}
 }
 for(let pass=0;pass<2;pass++) {
  await resize(800);
  await page.mouse.move(750,700);
  for(let i=0;i<12;i++){await page.mouse.wheel(0,500);await page.waitForTimeout(150);}
  await resize(1280);
  await expect.poll(async () => {
   const png=PNG.sync.read(await page.screenshot({clip:{x:750,y:200,width:450,height:600}}));
   const colors=new Set<number>();for(let i=0;i<png.data.length;i+=16)colors.add(png.data.readUInt32BE(i));
   return colors.size;
  }, {message:'Right column must paint content after reparenting'}).toBeGreaterThan(100);
  await page.screenshot({path:testInfo.outputPath(`returned-${pass}.png`)});
  const before=await page.screenshot({clip:{x:750,y:200,width:450,height:600}});
  // The deep-scroll setup may legitimately restore the last section at the end.
  // Move back into the list; another downward wheel can correctly be clamped.
  await page.mouse.move(1000,700);await page.mouse.wheel(0,-500);
  await expect.poll(async () => before.equals(await page.screenshot({clip:{x:750,y:200,width:450,height:600}})),
    {message:'Restored right column responds to upward scrolling'}).toBe(false);
 }
 expect(runtimeErrors).toEqual([]);
});
