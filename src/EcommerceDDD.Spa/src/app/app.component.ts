import { AfterViewInit, Component, Renderer2, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { LoaderService } from '@core/services/loader.service';
import { NavMenuComponent } from '@shared/components/nav-menu/nav-menu.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  
  imports: [CommonModule, RouterOutlet, NavMenuComponent],
})
export class AppComponent implements AfterViewInit {
  private loaderService = inject(LoaderService);
  private renderer = inject(Renderer2);

  title = 'ecommerceddd-spa';

  ngAfterViewInit() {
    this.loaderService.loading$.subscribe((status: boolean) => {
      if (status) {
        this.renderer.addClass(document.body, 'cursor-loader');
      } else {
        this.renderer.removeClass(document.body, 'cursor-loader');
      }
    });
  }
}
