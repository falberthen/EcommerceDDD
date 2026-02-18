import { Component, Renderer2, effect, inject } from '@angular/core';

import { RouterOutlet } from '@angular/router';
import { LoaderService } from '@core/services/loader.service';
import { NavMenuComponent } from '@shared/components/nav-menu/nav-menu.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  
  imports: [RouterOutlet, NavMenuComponent],
})
export class AppComponent {
  private loaderService = inject(LoaderService);
  private renderer = inject(Renderer2);

  title = 'ecommerceddd-spa';

  constructor() {
    effect(() => {
      const status = this.loaderService.loading();
      if (status) {
        this.renderer.addClass(document.body, 'cursor-loader');
      } else {
        this.renderer.removeClass(document.body, 'cursor-loader');
      }
    });
  }
}
