# EasyDI
Tuan's DI framework

Note: 

-Decore<> is Override Bind<>!!!!

  For Decore:

        Use:

          ContainerBinding.Decore<iSpeed>().To<buffSpeedInScene>().CustomGetInstance((a, b) => new buffSpeedInScene());
          ContainerBinding.Decore<iSpeed>().To<buffSpeed>().CustomGetInstance((a, b) => new iSpeed.Temp());

        Don't use :

          ContainerBinding.Decore<iSpeed>().To<buffSpeedInScene>().FromInstance(new buffSpeedInScene());
          ContainerBinding.Decore<iSpeed>().To<buffSpeed>().FromInstance(new iSpeed.Temp());

Because FromInstance make only 1 instance for all iSpeed when Inject, CustomGetInstance create corresponding instance foreach Inject!!!!

Install:

  -use UPM Git: 
  
    -package manager -> add from Git...
      https://github.com/wakawa2003/EasyDI-Core-upm.git
  
  or coppy Plugin/EasyDI folder to your project.
