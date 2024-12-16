# EasyDI
Tuan's DI framework

Note: 
  For Decore:

        Use:

          ContainerBinding.Decore<iSpeed>().To<buffSpeedInScene>().CustomGetInstance((a, b) => new buffSpeedInScene());
          ContainerBinding.Decore<iSpeed>().To<buffSpeed>().CustomGetInstance((a, b) => new iSpeed.Temp());

        Don't use :
        
          ContainerBinding.Decore<iSpeed>().To<buffSpeedInScene>().FromInstance(new buffSpeedInScene());
          ContainerBinding.Decore<iSpeed>().To<buffSpeed>().FromInstance(new iSpeed.Temp());

Because FromInstance make only 1 instance for all iSpeed when Inject, CustomGetInstance create corresponding instance foreach Inject!!!!
