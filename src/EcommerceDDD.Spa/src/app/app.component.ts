import { AfterViewInit, Component, Renderer2 } from '@angular/core';
import { LoaderService } from './core/services/loader.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements AfterViewInit {
  title = 'ecommerceddd-spa';

  constructor(
    private loaderService: LoaderService,
    private renderer: Renderer2
  ) {}

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
