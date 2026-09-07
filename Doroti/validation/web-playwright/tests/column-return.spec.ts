import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';
import { PNG } from 'pngjs';

for(const suffix of ['', ' @dpr']) test(`scrolled sample restores the right column after wide narrow wide${suffix}`, async ({page,runtimeErrors},testInfo)=>{
 await page.setViewportSize({width:1280,height:900});
 await openDoroti(page,'&dorotiTestbedMode=sample');
 for(const x of [600,1255]) {
  await page.mouse.move(x,700);
  for(let i=0;i<15;i++){await page.mouse.wheel(0,500);await page.waitForTimeout(150);}
 }
 for(let pass=0;pass<2;pass++) {
  await page.setViewportSize({width:800,height:900});await page.waitForTimeout(1200);
  await page.mouse.move(750,700);
  for(let i=0;i<12;i++){await page.mouse.wheel(0,500);await page.waitForTimeout(150);}
  await page.setViewportSize({width:1280,height:900});await page.waitForTimeout(1500);
  await page.screenshot({path:testInfo.outputPath(`returned-${pass}.png`)});
  const png=PNG.sync.read(await page.screenshot({clip:{x:750,y:200,width:450,height:600}}));
  const colors=new Set<number>();for(let i=0;i<png.data.length;i+=16)colors.add(png.data.readUInt32BE(i));
  expect(colors.size,'Right column must paint content after reparenting').toBeGreaterThan(100);
  const before=await page.screenshot({clip:{x:750,y:200,width:450,height:600}});
  await page.mouse.move(1255,700);await page.mouse.wheel(0,500);await page.waitForTimeout(500);
  expect(before.equals(await page.screenshot({clip:{x:750,y:200,width:450,height:600}})), 'Restored right column responds to scrolling').toBe(false);
 }
 expect(runtimeErrors).toEqual([]);
});
