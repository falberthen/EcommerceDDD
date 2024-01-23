import { Component } from '@angular/core';

@Component({
  selector: 'app-loader-skeleton',
  template: `
  <div class="loader">
  <div class="line"></div>
  <div class="line"></div>
  <div class="line"></div>
</div>`,
  styles: [`
    :host{
    .loader {
      display: flex;
      flex-direction: column;
    }
    
    .line {
      width: 90%;
      height: 20px;
      margin: 20px;
      background-color: #eee;
      border-radius: 4px;
    }
}`],
})
export class LoaderSkeletonComponent {}
