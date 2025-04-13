import { AfterViewInit, Component, Renderer2, inject } from '@angular/core';
import { LoaderService } from '@core/services/loader.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
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
