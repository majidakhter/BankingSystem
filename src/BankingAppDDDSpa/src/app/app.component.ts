import { Component, Renderer2, effect, inject, OnInit } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { LoaderService } from '@core/services/loader.service';
import { FooterComponent } from '@shared/footer/footer.component';
import { HeaderComponent } from '@shared/header/header.component';
import { MemberShipService } from '@core/services/membership.service';
import { filter } from 'rxjs/operators';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [RouterOutlet, HeaderComponent, FooterComponent],
})
export class AppComponent implements OnInit {
  private loaderService = inject(LoaderService);
  private renderer = inject(Renderer2);
  public authService = inject(MemberShipService);
  private router = inject(Router);

  currentUrl: string = '';

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

  ngOnInit(): void {
    this.currentUrl = this.router.url;
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentUrl = event.urlAfterRedirects || event.url;
    });
  }

  showLayoutHeaderFooter(): boolean {
    const isAuthPage = this.currentUrl.includes('/login') || this.currentUrl.includes('/register');
    return !isAuthPage && this.authService.isLoggedIn();
  }

  showAppHeader(): boolean {
    return this.showLayoutHeaderFooter();
  }
}

