
import { Component, DestroyRef, OnDestroy, OnInit, ViewContainerRef, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { Location } from '@angular/common';
import { Subscription } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MemberShipService } from '../../../core/services/membership.service';
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  imports: [RouterModule, FontAwesomeModule],
})
export class NavMenuComponent {
  

  }

  

